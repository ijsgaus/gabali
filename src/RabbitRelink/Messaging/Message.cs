namespace RabbitRelink.Messaging;

/// <summary>
/// Represents RabbitMQ message
/// </summary>
/// <param name="Body">Message body</param>
/// <param name="Properties">Message properties</param>
/// <typeparam name="TBody">Type of message body</typeparam>
public abstract record Message<TBody>(TBody Body, MessageProperties Properties) where TBody : class
{
    protected Message(TBody body) : this(body, new MessageProperties())
    {
    }
}
