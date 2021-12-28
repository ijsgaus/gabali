using RabbitRelink.Messaging;

namespace RabbitRelink.Middlewares;

/// <summary>
/// Signature of publish message function
/// </summary>
/// <typeparam name="T">message body type</typeparam>
public delegate Task DoPublish<T>(PublishMessage<T> msg,
    CancellationToken cancellation = default);
