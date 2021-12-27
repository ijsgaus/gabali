namespace RabbitRelink.Messaging;

/// <summary>
/// Represents RabbitMQ message for publish
/// </summary>
/// <param name="Body">Message body</param>
/// <param name="Properties">Message properties</param>
/// <param name="PublishProperties">Publish properties</param>
/// <typeparam name="TBody">ype of message body</typeparam>
public sealed record PublishMessage<TBody>(TBody Body, MessageProperties Properties, PublishProperties PublishProperties) : Message<TBody>(Body, Properties) where TBody : class?
{
    public PublishMessage(TBody body, PublishProperties publishProperties) : this(body, new MessageProperties(), publishProperties)
    {
    }
}
