using RabbitRelink.Consumer;

namespace RabbitRelink.Messaging;

/// <summary>
///  Represents RabbitMQ message received from broker
/// </summary>
/// <param name="Body">Body value</param>
/// <param name="Properties">Message properties</param>
/// <param name="ReceiveProperties">Receive properties</param>
/// <param name="Cancellation">Message cancellation</param>
public sealed record ConsumedMessage<TBody>(
        TBody Body,
        Properties Properties,
        ReceiveProperties ReceiveProperties,
        CancellationToken Cancellation)
    : Message<TBody>(Body, Properties)
{
    /// <summary>
    /// Convert to <see cref="PulledMessage{TBody}"/>
    /// </summary>
    /// <param name="source">Task completion source</param>
    /// <returns><see cref="PulledMessage{TBody}"/></returns>
    public PulledMessage<TBody> ToPulledMessage(TaskCompletionSource<Acknowledge> source) =>
        new(Body, Properties, ReceiveProperties, Cancellation, source);

    /// <summary>
    /// Change message body type
    /// </summary>
    /// <param name="mapper">mapper function</param>
    /// <typeparam name="TOther">other body type</typeparam>
    /// <returns><see cref="ConsumedMessage{TBody}"/> with <see cref="TOther"/> type</returns>
    public ConsumedMessage<TOther> Map<TOther>(Func<TBody, TOther> mapper)
        => new ConsumedMessage<TOther>(mapper(Body), Properties, ReceiveProperties, Cancellation);
}
