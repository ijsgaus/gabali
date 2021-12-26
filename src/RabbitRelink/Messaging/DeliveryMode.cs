namespace RabbitRelink.Messaging;

/// <summary>
/// Message delivery mode
/// </summary>
public enum DeliveryMode : byte
{
    /// <summary>
    /// Default
    /// </summary>
    Default = 0,

    /// <summary>
    /// Transient
    /// </summary>
    Transient = 1,

    /// <summary>
    /// Durable
    /// </summary>
    Persistent = 2
}
