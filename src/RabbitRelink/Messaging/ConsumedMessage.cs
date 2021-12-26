namespace RabbitRelink.Messaging;

/// <summary>
///  Represents RabbitMQ message received from broker
/// </summary>
/// <param name="Body">Body value</param>
/// <param name="Properties">Message properties</param>
/// <param name="ReceiveProperties">Receive properties</param>
/// <param name="Cancellation">Message cancellation</param>
public record ConsumedMessage<TBody>(TBody Body, MessageProperties Properties, ReceiveProperties ReceiveProperties,
    CancellationToken Cancellation) : Message<TBody>(Body, Properties) where TBody : class;
