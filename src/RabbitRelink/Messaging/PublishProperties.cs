namespace RabbitRelink.Messaging;

/// <summary>
/// Message publish properties
/// </summary>
public sealed record PublishProperties()
{
    private readonly string? _routingKey;

    /// <summary>
    /// Routing key
    /// </summary>
    public string? RoutingKey
    {
        get => _routingKey;
        init => _routingKey = string.IsNullOrWhiteSpace(value) ? null : value!.Trim();
    }

    /// <summary>
    /// Is message mandatory
    /// </summary>
    public bool? Mandatory { get; init; }
}
