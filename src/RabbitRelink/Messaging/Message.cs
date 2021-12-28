namespace RabbitRelink.Messaging;

/// <summary>
/// Represents RabbitMQ message
/// </summary>
/// <param name="Body">Message body</param>
/// <param name="Properties">Message properties</param>
/// <typeparam name="TBody">Type of message body</typeparam>
public abstract record Message<TBody>(TBody Body, Properties Properties)
{
    /// <summary>
    /// Short form constructor with default message properties
    /// </summary>
    /// <param name="body">message body</param>
    protected Message(TBody body) : this(body, new Properties())
    {
    }
}
