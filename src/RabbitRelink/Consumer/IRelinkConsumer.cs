#region Usings

#endregion

using RabbitRelink.Middlewares;

namespace RabbitRelink.Consumer
{
    /// <summary>
    ///     Represents RabbitMQ message consumer
    /// </summary>
    public interface IRelinkConsumer : IDisposable
    {
        /// <summary>
        ///     Consumer Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Consumer configuration
        /// </summary>
        PushConsumerConfig Config { get; }

        /// <summary>
        ///     Waits for consumer ready
        /// </summary>
        Task WaitReadyAsync(CancellationToken cancellation = default);
    }
}
