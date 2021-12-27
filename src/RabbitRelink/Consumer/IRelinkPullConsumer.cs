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
    public interface IRelinkPullConsumer : IDisposable
    {
        /// <summary>
        ///     Consumer Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Consumer configuration
        /// </summary>
        PullConsumerConfig Config { get; }

        /// <summary>
        ///     Waits for consumer ready
        /// </summary>
        Task WaitReadyAsync(CancellationToken? cancellation = null);

        /// <summary>
        /// Receive message channel reader
        /// </summary>
        ChannelReader<PulledMessage<byte[]>> Receiver { get; }
    }
}
