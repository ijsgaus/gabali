#region Usings

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitRelink.Connection;
using RabbitRelink.Internals;
using RabbitRelink.Internals.Async;
using RabbitRelink.Internals.Channels;
using RabbitRelink.Internals.Lens;
using RabbitRelink.Logging;
using RabbitRelink.Messaging;
using RabbitRelink.Topology;
using RabbitRelink.Topology.Internal;

#endregion

namespace RabbitRelink.Producer
{
    internal class RelinkProducer : AsyncStateMachine<RelinkProducerState>, IRelinkProducerInternal, IRelinkChannelHandler
    {
        #region Static fields

        private const string CorrelationHeader = "X-Producer-Corellation-Id";

        #endregion

        #region Fields

        private readonly ProducerAckQueue _ackQueue =
            new ProducerAckQueue();

        private readonly IRelinkChannel _channel;
        public ProducerConfig Config { get; }

        private readonly IRelinkLogger _logger;

        private readonly CompositeChannel<ProducerMessage<byte[]?>> _messageQueue =
            new(new LensChannel<ProducerMessage<byte[]?>>());

        private readonly object _sync = new object();

        private readonly TopologyRunner<IExchange> _topologyRunner;

        private IExchange? _exchange;

        private volatile TaskCompletionSource<object> _readyCompletion = new();

        private readonly string _appId;

        #endregion

        #region Ctor

        public RelinkProducer(
            IRelinkChannel channel,
            ProducerConfig config,
            Func<ITopologyCommander, Task<IExchange>> topologyConfig) : base(RelinkProducerState.Init)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Config = config;

            _logger = _channel.Connection.Config.LoggerFactory.CreateLogger($"{GetType().Name}({Id:D})");

            _topologyRunner =
                new TopologyRunner<IExchange>(_logger, topologyConfig);
            _appId = _channel.Connection.Config.AppId;

            _channel.Disposed += ChannelOnDisposed;

            _logger.Debug($"Created(channelId: {_channel.Id})");

            _channel.Initialize(this);
        }

        #endregion

        #region ILinkChannelHandler Members

        public async Task OnActive(IModel model, CancellationToken cancellation)
        {
            var newState = RelinkProducerState.Init;

            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    newState = RelinkProducerState.Stopping;
                }

                ChangeState(newState);

                switch (State)
                {
                    case RelinkProducerState.Init:
                        newState = RelinkProducerState.Configuring;
                        break;
                    case RelinkProducerState.Configuring:
                    case RelinkProducerState.Reconfiguring:
                        newState = await ConfigureAsync(
                                model,
                                State == RelinkProducerState.Reconfiguring,
                                cancellation
                            )
                            .ConfigureAwait(false)
                            ? RelinkProducerState.Active
                            : RelinkProducerState.Reconfiguring;
                        break;
                    case RelinkProducerState.Active:
                        try
                        {
                            await ActiveAsync(model, cancellation)
                                .ConfigureAwait(false);
                            newState = RelinkProducerState.Stopping;
                        }
                        finally
                        {
                            if (_readyCompletion.Task.IsCompleted)
                                _readyCompletion = new TaskCompletionSource<object>();
                        }
                        break;
                    case RelinkProducerState.Stopping:
                        await AsyncHelper.RunAsync(Stop)
                            .ConfigureAwait(false);

                        if (cancellation.IsCancellationRequested)
                        {
                            ChangeState(RelinkProducerState.Init);
                            return;
                        }
                        newState = RelinkProducerState.Reconfiguring;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(State), $"Handler for state ${State} not implemented");
                }
            }
        }



        protected override void OnStateChange(RelinkProducerState newState)
        {
            _logger.Debug($"State change {State} -> {newState}");

            try
            {
                Config.OnStateChanged(State, newState);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Exception in state handler: {ex}");
            }

            base.OnStateChange(newState);
        }

        public async Task OnConnecting(CancellationToken cancellation)
        {
            if(cancellation.IsCancellationRequested)
                return;

            try
            {
                await _messageQueue.YieldAsync(cancellation)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // No op
            }
            catch (OperationCanceledException)
            {
                // No op
            }
        }

        public void MessageAck(BasicAckEventArgs info)
        {
            _ackQueue.Ack(info.DeliveryTag, info.Multiple);
        }

        public void MessageNack(BasicNackEventArgs info)
        {
            _ackQueue.Nack(info.DeliveryTag, info.Multiple);
        }

        public void MessageReturn(BasicReturnEventArgs info)
        {
            if (info.BasicProperties.Headers.TryGetValue(CorrelationHeader, out var correlationValue))
            {
                try
                {
                    var correlationId = correlationValue is string value
                        ? value
                        : correlationValue is byte[] bytes
                            ? Encoding.UTF8.GetString(bytes)
                            : null;
                    if (correlationId != null)
                    {
                        _ackQueue.Return(correlationId, info.ReplyText);
                    }
                }
                catch
                {
                    // no-op
                }
            }
        }

        #endregion

        #region ILinkProducerInternal Members

        public event EventHandler? Disposed;
        public IRelinkChannel Channel => _channel;

        public void Dispose()
        {
            Dispose(false);
        }

        public Task WaitReadyAsync(CancellationToken cancellation = default)
        {
            return _readyCompletion.Task
                .ContinueWith(
                    t => t.Result,
                    cancellation,
                    TaskContinuationOptions.RunContinuationsAsynchronously,
                    TaskScheduler.Current
                );
        }

        public Task PublishAsync(byte[]? body, Func<Properties, Properties>? configureProperties = null, Func<PublishProperties, PublishProperties>? configurePublish = null,
            CancellationToken cancellation = default)
        {
            if (State == RelinkProducerState.Disposed)
                throw new ObjectDisposedException(GetType().Name);
            var publishProps = new PublishProperties();
            publishProps = Config.UpdatePublish(publishProps);
            publishProps = (configurePublish ?? (p => p))(publishProps);


            if (publishProps.Mandatory == true && !Config.ConfirmsMode)
                throw new NotSupportedException("Mandatory without ConfirmsMode not supported");

            var msgProperties = new Properties
            {
                AppId = _appId,
                UserId = _channel.Connection.UserId,
            };
            msgProperties = Config.IdGenerator.SetMessageId(
                body,
                msgProperties,
                publishProps
            );
            msgProperties = Config.UpdateProperties(msgProperties);
            msgProperties = (configureProperties ?? (p => p))(msgProperties);



            var msg = new ProducerMessage<byte[]?>(body, msgProperties, publishProps)
            {
                Cancellation = cancellation
            };

            try
            {
                _messageQueue.Put(msg);
            }
            catch
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return msg.Completion;
        }

        public Guid Id { get; } = Guid.NewGuid();


        #endregion

        private void Stop()
        {
            var messages = _ackQueue.Reset();

            if (messages.Count > 0)
            {
                _logger.Warning($"Requeue {messages.Count} not ACKed or NACKed messages");
            }

            _messageQueue.PutRetry(messages, CancellationToken.None);
        }

        private async Task ActiveAsync(IModel model, CancellationToken cancellation)
        {
            try
            {
                await SetConfirmsModeAsync(model)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Confirms mode activation error: {ex}");
                return;
            }

            _readyCompletion.TrySetResult(null!);
            await AsyncHelper.RunAsync(() => ProcessQueue(model, cancellation))
                .ConfigureAwait(false);
        }

        private void ProcessQueue(IModel model, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                ProducerMessage<byte[]?> message;
                try
                {
                    message = _messageQueue.Wait(cancellation);
                }
                catch (Exception ex)
                {
                    if (cancellation.IsCancellationRequested)
                        continue;

                    _logger.Error($"Cannot read message from queue: {ex}");
                    return;
                }

                var seq = model.NextPublishSeqNo;
                var correlationId = _ackQueue.Add(message, seq);

                var properties = model.CreateBasicProperties();
                if (message.Properties.Expiration != null)
                    properties.Expiration = ((long) message.Properties.Expiration.Value.TotalMilliseconds).ToString();
                properties.Headers = message.Properties.Headers?.ToDictionary(p => p.Key,  p => p.Value);
                if (message.Properties.DeliveryMode != DeliveryMode.Default) properties.DeliveryMode = (byte) message.Properties.DeliveryMode;
                if (message.Properties.AppId != null) properties.AppId = message.Properties.AppId;
                if (message.Properties.ClusterId != null) properties.ClusterId = message.Properties.ClusterId;
                if (message.Properties.ContentEncoding != null) properties.ContentEncoding = message.Properties.ContentEncoding;
                if (message.Properties.ContentType != null) properties.ContentType = message.Properties.ContentType;
                if (message.Properties.CorrelationId != null) properties.CorrelationId = message.Properties.CorrelationId;
                if (message.Properties.ReplyTo != null) properties.ReplyTo = message.Properties.ReplyTo;
                if (message.Properties.MessageId != null) properties.MessageId = message.Properties.MessageId;
                if (message.Properties.TimeStamp != null) properties.Timestamp = new AmqpTimestamp(message.Properties.TimeStamp.Value);
                if (message.Properties.Type != null) properties.Type = message.Properties.Type;
                if (message.Properties.UserId != null) properties.UserId = message.Properties.UserId;
                if (message.Properties.Priority != null) properties.Priority = message.Properties.Priority.Value;

                if (message.Properties.UserId != null)
                    properties.UserId = message.Properties.UserId;

                properties.Headers ??= new Dictionary<string, object>();

                properties.Headers[CorrelationHeader] = Encoding.UTF8.GetBytes(correlationId);

                try
                {
                    model.BasicPublish(
                        _exchange!.Name,
                        message.PublishProperties.RoutingKey ?? "",
                        message.PublishProperties.Mandatory ?? false,
                        properties,
                        message.Body
                    );
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot publish message: {ex.Message}");
                    return;
                }

                if (!Config.ConfirmsMode)
                {
                    _ackQueue.Ack(seq, false);
                }
            }
        }

        private Task SetConfirmsModeAsync(IModel model)
        {
            if (Config.ConfirmsMode)
            {
                return AsyncHelper.RunAsync(model.ConfirmSelect);
            }

            return Task.CompletedTask;
        }

        private async Task<bool> ConfigureAsync(IModel model, bool retry, CancellationToken cancellation)
        {
            if (retry)
            {
                try
                {
                    _logger.Debug($"Retrying in {Config.RecoveryInterval.TotalSeconds:0.###}s");
                    await Task.Delay(Config.RecoveryInterval, cancellation)
                        .ConfigureAwait(false);
                }
                catch
                {
                    return false;
                }
            }

            _logger.Debug("Configuring topology");

            try
            {
                _exchange = await _topologyRunner
                    .RunAsync(model, cancellation)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Exception on topology configuration: {ex}");
                return false;
            }

            _logger.Debug("Topology configured");

            return true;
        }

        private void ChannelOnDisposed(object? sender, EventArgs eventArgs)
            => Dispose(true);

        private void Dispose(bool byChannel)
        {
            if (State == RelinkProducerState.Disposed)
                return;

            lock (_sync)
            {
                if (State == RelinkProducerState.Disposed)
                    return;

                _logger.Debug($"Disposing ( by channel: {byChannel} )");

                _channel.Disposed -= ChannelOnDisposed;
                if (!byChannel)
                {
                    _channel.Dispose();
                }

                var ex = new ObjectDisposedException(GetType().Name);
                _messageQueue.Dispose();

                ChangeState(RelinkProducerState.Disposed);

                _readyCompletion.TrySetException(ex);

                _logger.Debug("Disposed");
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
