#region Usings

using RabbitRelink.Messaging;

#endregion

namespace RabbitRelink.Producer;

/// <summary>
/// Represents RabbitMQ message producer
/// </summary>
public interface IRelinkProducer<T> : IDisposable where T: class
{
    #region Properties

    /// <summary>
    /// Producer Id
    /// </summary>
    Guid Id { get; }

    ProducerConfig Config { get; }

    /// <summary>
    /// Operational state
    /// </summary>
    RelinkProducerState State { get; }

    #endregion

    /// <summary>
    /// Waits for producer ready
    /// </summary>
    Task WaitReadyAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Publishes RAW message
    /// </summary>
    /// <param name="configurePublish">configure <see cref="PublishProperties"/></param>
    /// <param name="cancellation">cancellation token, if null <see cref="Timeout" /> will be used</param>
    /// <param name="body">message body</param>
    /// <param name="configureProperties">configure <see cref="MessageProperties"/></param>
    /// <returns><see cref="Task" /> which completed when message ACKed by broker</returns>
    Task PublishAsync(T body, Func<MessageProperties, MessageProperties>? configureProperties = null,
        Func<PublishProperties, PublishProperties>? configurePublish = null,
        CancellationToken cancellation = default
    );
}
