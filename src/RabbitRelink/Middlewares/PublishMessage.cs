using RabbitRelink.Messaging;

namespace RabbitRelink.Middlewares;

public delegate Task PublishMessage<T>(T body, Func<MessageProperties, MessageProperties>? configureProperties = null,
    Func<PublishProperties, PublishProperties>? configurePublish = null,
    CancellationToken cancellation = default);
