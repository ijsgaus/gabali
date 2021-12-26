using System.Collections.Immutable;
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

namespace RabbitRelink.Consumer
{
    internal class RelinkConsumer : AsyncStateMachine<RelinkConsumerState>, IRelinkConsumerInternal, IRelinkChannelHandler
    {
        private readonly RelinkConsumerConfig _config;
        private readonly Func<ConsumedMessage<byte[]>, Task<Acknowledge>> _handler;
        private readonly IRelinkChannel _channel;
        private readonly IRelinkLogger _logger;

        private readonly object _sync = new object();

        private readonly TopologyRunner<IQueue> _topologyRunner;
        private IQueue? _queue;

        private volatile TaskCompletionSource<object> _readyCompletion =
            new TaskCompletionSource<object>();

        private readonly CompositeChannel<RelinkConsumerMessageAction> _actionQueue =
            new CompositeChannel<RelinkConsumerMessageAction>(new LensChannel<RelinkConsumerMessageAction>());

        private volatile EventingBasicConsumer? _consumer;
        private volatile CancellationTokenSource? _consumerCancellationTokenSource;

        private readonly string _appId;

        public RelinkConsumer(
            RelinkConsumerConfig config,
            IRelinkChannel channel,
            Func<ITopologyCommander, Task<IQueue>> topologyHandler,
            Func<ConsumedMessage<byte[]>, Task<Acknowledge>> handler) : base(RelinkConsumerState.Init)
        {
            _config = config;

            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _handler = handler;

            _logger = _channel.Connection.Config.LoggerFactory.CreateLogger($"{GetType().Name}({Id:D})")
                      ?? throw new InvalidOperationException("Cannot create logger");

            _topologyRunner = new TopologyRunner<IQueue>(_logger, topologyHandler);
            _appId = _channel.Connection.Config.AppId;

            _channel.Disposed += ChannelOnDisposed;

            _channel.Initialize(this);
        }

        public Guid Id { get; } = Guid.NewGuid();
        public ushort PrefetchCount => _config.PrefetchCount;
        public bool AutoAck => _config.AutoAck;
        public int? Priority => _config.Priority;
        public bool CancelOnHaFailover => _config.CancelOnHaFailover;
        public bool Exclusive => _config.Exclusive;

        public Task WaitReadyAsync(CancellationToken? cancellation = null)
        {
            return _readyCompletion.Task
                .ContinueWith(
                    t => t.Result,
                    cancellation ?? CancellationToken.None,
                    TaskContinuationOptions.RunContinuationsAsynchronously,
                    TaskScheduler.Current
                );
        }

        public event EventHandler? Disposed;
        public IRelinkChannel Channel => _channel;

        private void ChannelOnDisposed(object? sender, EventArgs eventArgs)
            => Dispose(true);

        public void Dispose()
            => Dispose(false);

        private void Dispose(bool byChannel)
        {
            if (State == RelinkConsumerState.Disposed)
                return;

            lock (_sync)
            {
                if (State == RelinkConsumerState.Disposed)
                    return;

                _logger.Debug($"Disposing ( by channel: {byChannel} )");

                _channel.Disposed -= ChannelOnDisposed;
                if (!byChannel)
                {
                    _channel.Dispose();
                }

                var ex = new ObjectDisposedException(GetType().Name);

                ChangeState(RelinkConsumerState.Disposed);

                _readyCompletion.TrySetException(ex);

                _logger.Debug("Disposed");

                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnStateChange(RelinkConsumerState newState)
        {
            _logger.Debug($"State change {State} -> {newState}");

            try
            {
                _config.StateChanged(State, newState);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Exception in state handler: {ex}");
            }

            base.OnStateChange(newState);
        }

        public async Task OnActive(IModel model, CancellationToken cancellation)
        {
            var newState = RelinkConsumerState.Init;

            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    newState = RelinkConsumerState.Stopping;
                }

                ChangeState(newState);

                switch (State)
                {
                    case RelinkConsumerState.Init:
                        newState = RelinkConsumerState.Configuring;
                        break;
                    case RelinkConsumerState.Configuring:
                    case RelinkConsumerState.Reconfiguring:
                        newState = await ConfigureAsync(
                                model,
                                State == RelinkConsumerState.Reconfiguring,
                                cancellation
                            )
                            .ConfigureAwait(false)
                            ? RelinkConsumerState.Active
                            : RelinkConsumerState.Reconfiguring;
                        break;
                    case RelinkConsumerState.Active:
                        await ActiveAsync(model, cancellation)
                            .ConfigureAwait(false);

                        newState = RelinkConsumerState.Stopping;
                        break;
                    case RelinkConsumerState.Stopping:
                        await AsyncHelper.RunAsync(() => Stop(model))
                            .ConfigureAwait(false);

                        if (cancellation.IsCancellationRequested)
                        {
                            ChangeState(RelinkConsumerState.Init);
                            return;
                        }

                        newState = RelinkConsumerState.Reconfiguring;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(State), $"Handler for state ${State} not implemented");
                }
            }
        }

        private async Task<bool> ConfigureAsync(IModel model, bool retry, CancellationToken cancellation)
        {
            if (retry)
            {
                try
                {
                    _logger.Debug($"Retrying in {_config.RecoveryInterval.TotalSeconds:0.###}s");
                    await Task.Delay(_config.RecoveryInterval, cancellation)
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
                _queue = await _topologyRunner
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

        private async Task ActiveAsync(IModel model, CancellationToken cancellation)
        {
            try
            {
                await AsyncHelper.RunAsync(() => InitializeConsumer(model, cancellation))
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannot initialize: {ex}");
                return;
            }

            using var ccs = CancellationTokenSource
                .CreateLinkedTokenSource(cancellation, _consumerCancellationTokenSource?.Token ?? throw new NullReferenceException($"{_consumerCancellationTokenSource} not initialized"));
            var token = ccs.Token;

            try
            {
                _readyCompletion.TrySetResult(new object());

                await AsyncHelper.RunAsync(() => ProcessActionQueue(model, token))
                    .ConfigureAwait(false);
            }
            catch
            {
                // no-op
            }
            finally
            {
                if (_readyCompletion.Task.IsCompleted)
                    _readyCompletion = new TaskCompletionSource<object>();
            }
        }

        private void ProcessActionQueue(IModel model, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                RelinkConsumerMessageAction action;
                try
                {
                    action = _actionQueue.Wait(cancellation);
                }
                catch (Exception ex)
                {
                    if (cancellation.IsCancellationRequested)
                        continue;

                    _logger.Error($"Cannot read message from action queue: {ex}");
                    return;
                }

                try
                {
                    switch (action.Strategy)
                    {
                        case Acknowledge.Ack:
                            model.BasicAck(action.Seq, false);
                            break;
                        case Acknowledge.Nack:
                        case Acknowledge.Requeue:
                            model.BasicNack(action.Seq, false, action.Strategy == Acknowledge.Requeue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(action.Strategy), $"AckStrategy {action.Strategy} not supported");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot publish message: {ex.Message}");
                    return;
                }
            }
        }

        private void InitializeConsumer(IModel model, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            _consumerCancellationTokenSource = new CancellationTokenSource();

            _consumer = new EventingBasicConsumer(model);
            _consumer.Received += ConsumerOnReceived;
            _consumer.Registered += ConsumerOnRegistered;
            _consumer.ConsumerCancelled += ConsumerOnConsumerCancelled;

            cancellation.ThrowIfCancellationRequested();

            model.BasicQos(0, PrefetchCount, false);

            cancellation.ThrowIfCancellationRequested();

            var options = new Dictionary<string, object>();


            if (Priority != null && Priority != 0)
                options["x-priority"] = Priority;

            if (CancelOnHaFailover)
                options["x-cancel-on-ha-failover"] = CancelOnHaFailover;

            model.BasicConsume(_queue!.Name, AutoAck, Id.ToString("D"), false, Exclusive, options, _consumer);
        }

        private void ConsumerOnRegistered(object? sender, ConsumerEventArgs e)
            => _logger.Debug($"Consuming: {string.Join(", ", e.ConsumerTags)}");


        private void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var props = new MessageProperties
                {
                    AppId = e.BasicProperties.AppId,
                    ClusterId = e.BasicProperties.ClusterId,
                    ContentEncoding = e.BasicProperties.ContentEncoding,
                    CorrelationId = e.BasicProperties.CorrelationId,
                    DeliveryMode = (DeliveryMode) e.BasicProperties.DeliveryMode,
                    ReplyTo = e.BasicProperties.ReplyTo,
                    Expiration = e.BasicProperties.Expiration == null
                        ? null
                        : TimeSpan.FromMilliseconds(long.Parse(e.BasicProperties.Expiration)),
                    MessageId = e.BasicProperties.MessageId,
                    TimeStamp = e.BasicProperties.Timestamp.UnixTime,
                    Type = e.BasicProperties.Type,
                    UserId = e.BasicProperties.UserId,
                    Priority = e.BasicProperties.Priority,
                    Headers = e.BasicProperties.Headers?.ToImmutableDictionary()
                };


                var receiveProps = new ReceiveProperties(e.Redelivered, e.Exchange, e.RoutingKey, _queue?.Name ?? "",
                    props.AppId == _appId);

                var token = _consumerCancellationTokenSource?.Token ?? throw new NullReferenceException($"{_consumerCancellationTokenSource} not initialized");

                var msg = new ConsumedMessage<byte[]>(e.Body.ToArray(), props, receiveProps, token);

                HandleMessageAsync(msg, e.DeliveryTag);
            }
            catch (Exception ex)
            {
                _logger.Error($"Receive message error, NACKing: {ex}");

                try
                {
                    _actionQueue.Put(new RelinkConsumerMessageAction(
                        e.DeliveryTag,
                        Acknowledge.Nack,
                        _consumerCancellationTokenSource!.Token)
                    );
                }
                catch
                {
                    // No-op
                }
            }
        }

        private void HandleMessageAsync(ConsumedMessage<byte[]> msg, ulong deliveryTag)
        {
            var cancellation = msg.Cancellation;

            Task<Acknowledge> task;

            try
            {
                task = _handler(msg);
            }
            catch (Exception ex)
            {
                task = Task.FromException<Acknowledge>(ex);
            }

            task.ContinueWith(
                t => OnMessageHandledAsync(t, deliveryTag, cancellation),
                cancellation,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Current
            );
        }

        private async Task OnMessageHandledAsync(Task<Acknowledge> task, ulong deliveryTag,
            CancellationToken cancellation)
        {
            if (AutoAck) return;

            try
            {
                RelinkConsumerMessageAction action;

                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        action = new RelinkConsumerMessageAction(deliveryTag, task.Result, cancellation);
                        break;
                    case TaskStatus.Faulted:
                        var taskEx = task.Exception!.GetBaseException();
                        action = new RelinkConsumerMessageAction(deliveryTag, Acknowledge.Nack, cancellation);
                        _logger.Warning($"Error in MessageHandler (strategy: {action.Strategy}): {taskEx}");
                        break;
                    case TaskStatus.Canceled:
                        action = new RelinkConsumerMessageAction(deliveryTag, Acknowledge.Requeue, cancellation);
                        _logger.Warning($"MessageHandler cancelled (strategy: {action.Strategy})");
                        break;
                    default:
                        return;
                }

                await _actionQueue.PutAsync(action)
                    .ConfigureAwait(false);
            }
            catch
            {
                //no-op
            }
        }

        private void ConsumerOnConsumerCancelled(object? sender, ConsumerEventArgs e)
        {
            _logger.Debug($"Cancelled: {string.Join(", ", e.ConsumerTags)}");
            _consumerCancellationTokenSource?.Cancel();
            _consumerCancellationTokenSource?.Dispose();
        }

        private void Stop(IModel model)
        {
            if (_consumer != null)
            {
                try
                {
                    if (_consumer.IsRunning)
                    {
                        model.BasicCancel(_consumer.ConsumerTags.First());
                    }
                }
                catch
                {
                    //No-op
                }
                finally
                {
                    _consumer = null;
                }
            }
        }

        public async Task OnConnecting(CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
                return;

            try
            {
                await _actionQueue.YieldAsync(cancellation)
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
            // no-op
        }

        public void MessageNack(BasicNackEventArgs info)
        {
            // no-op
        }

        public void MessageReturn(BasicReturnEventArgs info)
        {
            // no-op
        }
    }
}
