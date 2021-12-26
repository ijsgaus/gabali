#region Usings

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitRelink.Connection;
using RabbitRelink.Internals;
using RabbitRelink.Internals.Async;
using RabbitRelink.Logging;

#endregion

namespace RabbitRelink.Topology.Internal
{
    internal class RelinkTopology : AsyncStateMachine<LinkTopologyState>, IRelinkTopologyInternal, IRelinkChannelHandler
    {
        #region Fields

        private readonly IRelinkChannel _channel;
        private readonly RelinkTopologyConfig _config;
        private readonly IRelinkLogger _logger;
        private readonly object _sync = new object();
        private readonly TopologyRunner<object> _topologyRunner;

        private volatile TaskCompletionSource<object?> _readyCompletion = new();

        #endregion

        #region Ctor

        public RelinkTopology(IRelinkChannel channel, RelinkTopologyConfig config, Func<ITopologyCommander, Task> handler)
            : base(LinkTopologyState.Init)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _config = config;

            _logger = _channel.Connection.Config.LoggerFactory.CreateLogger($"{GetType().Name}({Id:D})")
                      ?? throw new InvalidOperationException("Cannot create logger");

            _topologyRunner = new TopologyRunner<object>(_logger, async cfg =>
            {
                await handler(cfg).ConfigureAwait(false);
                return null;
            });

            _channel.Disposed += ChannelOnDisposed;

            _logger.Debug($"Created(channelId: {_channel.Id})");

            _channel.Initialize(this);
        }

        #endregion

        #region ILinkChannelHandler Members

        public async Task OnActive(IModel model, CancellationToken cancellation)
        {
            var newState = State;

            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    newState = LinkTopologyState.Stopping;
                }

                ChangeState(newState);

                switch (State)
                {
                    case LinkTopologyState.Init:
                        newState = LinkTopologyState.Configuring;
                        break;
                    case LinkTopologyState.Configuring:
                    case LinkTopologyState.Reconfiguring:
                        newState = await OnConfigureAsync(model, State == LinkTopologyState.Reconfiguring,
                                cancellation)
                            .ConfigureAwait(false);
                        break;
                    case LinkTopologyState.Ready:
                        _readyCompletion.TrySetResult(null);

                        try
                        {
                            await cancellation.WaitCancellation()
                                .ConfigureAwait(false);
                        }
                        finally
                        {
                            _readyCompletion =
                                new TaskCompletionSource<object?>();
                        }

                        newState = LinkTopologyState.Stopping;
                        break;
                    case LinkTopologyState.Disposed:
#pragma warning disable 4014
                        Task.Factory.StartNew(Dispose, TaskCreationOptions.LongRunning);
#pragma warning restore 4014
                        return;
                    case LinkTopologyState.Stopping:
                        if (cancellation.IsCancellationRequested)
                        {
                            ChangeState(LinkTopologyState.Init);
                            return;
                        }
                        return;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(State), $"Handler for state ${State} not implemented");
                }
            }
        }

        protected override void OnStateChange(LinkTopologyState newState)
        {
            _logger.Debug($"State change {State} -> {newState}");

            base.OnStateChange(newState);
        }

        public Task OnConnecting(CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }

        public void MessageAck(BasicAckEventArgs info)
        {
        }

        public void MessageNack(BasicNackEventArgs info)
        {
        }

        public void MessageReturn(BasicReturnEventArgs info)
        {
        }

        #endregion

        #region ILinkTopology Members

        public event EventHandler? Disposed;


        public void Dispose()
        {
            Dispose(false);
        }


        public Guid Id { get; } = Guid.NewGuid();

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

        #endregion

        private async Task<LinkTopologyState> OnConfigureAsync(IModel model, bool retry, CancellationToken cancellation)
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
                    return LinkTopologyState.Reconfiguring;
                }
            }

            _logger.Debug("Configuring topology");

            try
            {
                await _topologyRunner
                    .RunAsync(model, cancellation)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Exception on configuration: {ex}");
                return LinkTopologyState.Reconfiguring;
            }

            _logger.Debug("Topology configured");

            return LinkTopologyState.Ready;
        }

        private void Dispose(bool byChannel)
        {
            if (State == LinkTopologyState.Disposed)
                return;

            lock (_sync)
            {
                if (State == LinkTopologyState.Disposed)
                    return;

                _logger.Debug($"Disposing ( by channel: {byChannel} )");

                _channel.Disposed -= ChannelOnDisposed;
                if (!byChannel)
                {
                    _channel.Dispose();
                }

                ChangeState(LinkTopologyState.Disposed);

                _readyCompletion.TrySetException(new ObjectDisposedException(GetType().Name));

                _logger.Debug("Disposed");

                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }


        private void ChannelOnDisposed(object? sender, EventArgs eventArgs)
        {
            Dispose(true);
        }
    }
}
