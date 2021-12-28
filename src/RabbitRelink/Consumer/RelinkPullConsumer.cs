#region Usings

#endregion

using System.Collections.Immutable;
using System.Threading.Channels;
using RabbitRelink.Connection;
using RabbitRelink.Messaging;
using RabbitRelink.Middlewares;
using RabbitRelink.Topology;

namespace RabbitRelink.Consumer
{
    internal class RelinkPullConsumer<T> : IRelinkPullConsumer<T> where T: class?
    {
        public PullConsumerConfig Config { get; }

        #region Fields

        private readonly IRelinkConsumer _consumer;
        private readonly object _sync = new object();
        private bool _disposed;
        private readonly Channel<PulledMessage<T>> _channel;

        #endregion

        #region Ctor

        public RelinkPullConsumer(PullConsumerConfig config, Func<PushConsumerConfig, DoConsume<T>, IRelinkConsumer> factory)
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
                ? Channel.CreateUnbounded<PulledMessage<T>>()
                : Channel.CreateBounded<PulledMessage<T>>(Config.BoundChannel);
            DoConsume<T> handler = OnMessageReceived;
            _consumer = factory(pushConfig, handler);
        }

        #endregion

        public void Dispose()
        {

           _consumer.Dispose();
        }

        public Guid Id => _consumer.Id;

        public Task WaitReadyAsync(CancellationToken cancellation = default)
            => _consumer.WaitReadyAsync(cancellation);

        public ChannelReader<PulledMessage<T>> Receiver => _channel.Reader;

        private void OnStateChanged(RelinkConsumerState oldState, RelinkConsumerState newsState)
        {
            if (newsState == RelinkConsumerState.Disposed)
            {
                OnDispose();
            }

            Config.StateChanged(oldState, newsState);
        }

        private async Task<Acknowledge> OnMessageReceived(ConsumedMessage<T> message)
        {
            var completeSource = new TaskCompletionSource<Acknowledge>();
            var msg = new PulledMessage<T>(message, completeSource);
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
