namespace RabbitRelink.Messaging;

/// <summary>
/// MessageId generator
/// </summary>
public interface IMessageIdGenerator
{
    /// <summary>
    /// Set message id
    /// </summary>
    MessageProperties SetMessageId(byte[] body, MessageProperties properties, PublishProperties publishProperties);
}
