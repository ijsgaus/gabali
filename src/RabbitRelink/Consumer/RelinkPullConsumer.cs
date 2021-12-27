#region Usings

#endregion

using System.Threading.Channels;
using RabbitRelink.Connection;
using RabbitRelink.Messaging;
using RabbitRelink.Topology;

namespace RabbitRelink.Consumer
{
    internal class RelinkPullConsumer : IRelinkPullConsumer
    {
        public PullConsumerConfig Config { get; }

        #region Fields

        private readonly IRelinkConsumer _consumer;
        private readonly object _sync = new object();
        private bool _disposed;
        private readonly Channel<PulledMessage<byte[]>> _channel;

        #endregion

        #region Ctor

        public RelinkPullConsumer(
            PullConsumerConfig config,
            IRelinkChannel channel,
            Func<ITopologyCommander, Task<IQueue>> topologyHandler
        )
        {
            Config = config;
            var pushConfig = new PushConsumerConfig
            {
                Exclusive = Config.Exclusive,
                Parallelism = PushConsumerConfig.PARALLELISM_FULL,
                Priority = Config.Priority,
                AutoAck = Config.AutoAck,
                PrefetchCount = Config.PrefetchCount,
                RecoveryInterval = Config.RecoveryInterval,
                StateChanged = OnStateChanged,
                ChannelStateChanged = Config.ChannelStateChanged,
                CancelOnHaFailover = Config.CancelOnHaFailover,
            };
            _channel = Config.BoundChannel == PullConsumerConfig.UNBOUND
                ? Channel.CreateUnbounded<PulledMessage<byte[]>>()
                : Channel.CreateBounded<PulledMessage<byte[]>>(Config.BoundChannel);
            _consumer = new RelinkConsumer(pushConfig, channel, topologyHandler, OnMessageReceived);
        }

        #endregion

        public void Dispose()
        {

           _consumer.Dispose();
        }

        public Guid Id => _consumer.Id;

        public Task WaitReadyAsync(CancellationToken? cancellation = null)
            => _consumer.WaitReadyAsync(cancellation);

        public ChannelReader<PulledMessage<byte[]>> Receiver => _channel.Reader;

        private void OnStateChanged(RelinkConsumerState oldState, RelinkConsumerState newsState)
        {
            if (newsState == RelinkConsumerState.Disposed)
            {
                OnDispose();
            }

            Config.StateChanged(oldState, newsState);
        }

        private async Task<Acknowledge> OnMessageReceived(ConsumedMessage<byte[]> message)
        {
            var completeSource = new TaskCompletionSource<Acknowledge>();
            var msg = new PulledMessage<byte[]>(message, completeSource);
            await _channel.Writer.WriteAsync(msg, msg.Cancellation);
            return await completeSource.Task;
        }

        private void OnDispose()
        {
            if (_disposed)
                return;

            lock (_sync)
            {
                if (_disposed)
                    return;

                _channel.Writer.TryComplete();
                _disposed = true;
            }
        }
    }
}
