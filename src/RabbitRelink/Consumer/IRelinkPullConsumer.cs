#region Usings

#endregion

using System.Threading.Channels;
using RabbitRelink.Messaging;

namespace RabbitRelink.Consumer
{
    /// <summary>
    ///     Represents RabbitMQ message consumer which manage internal message queue
    ///     and implements semantic
    /// </summary>
    public interface IRelinkPullConsumer<T> : IDisposable where T: class?
    {
        /// <summary>
        /// Consumer Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Consumer configuration
        /// </summary>
        PullConsumerConfig Config { get; }

        /// <summary>
        ///     Waits for consumer ready
        /// </summary>
        Task WaitReadyAsync(CancellationToken cancellation = default);

        /// <summary>
        /// Receive message channel reader
        /// </summary>
        ChannelReader<PulledMessage<T>> Receiver { get; }
    }
}
