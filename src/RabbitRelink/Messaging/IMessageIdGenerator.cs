namespace RabbitRelink.Messaging;

/// <summary>
/// MessageId generator
/// </summary>
public interface IMessageIdGenerator
{
    /// <summary>
    /// Set message id
    /// </summary>
    Properties SetMessageId(byte[]? body, Properties properties, PublishProperties publishProperties);
}
