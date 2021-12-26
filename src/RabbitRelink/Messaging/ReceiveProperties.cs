namespace RabbitRelink.Messaging;

/// <summary>
/// Receive properties of message
/// </summary>
/// <param name="Redelivered">Is message was redelivered</param>
/// <param name="ExchangeName">Message was published to this exchange</param>
/// <param name="RoutingKey">Message was published with this routing key</param>
/// <param name="QueueName">Message was consumed from this queue</param>
/// <param name="IsFromThisApp">Message was published from this application</param>
public sealed record ReceiveProperties(
    bool Redelivered,
    string ExchangeName,
    string? RoutingKey,
    string QueueName,
    bool IsFromThisApp
);
