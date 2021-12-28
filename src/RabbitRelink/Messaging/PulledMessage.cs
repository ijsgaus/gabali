#region Usings

#endregion

using RabbitMQ.Client.Events;
using RabbitRelink.Consumer;

namespace RabbitRelink.Messaging;

/// <summary>
/// Received message for pull consumer <see cref="IRelinkPullConsumer{T}"/>
/// </summary>
/// <param name="Body">message body</param>
/// <param name="Properties">message properties</param>
/// <param name="ReceiveProperties">receive properties</param>
/// <param name="Cancellation">cancellation</param>
/// <param name="Completion"><see cref="TaskCompletionSource{T}"/> for <see cref="Acknowledge"/>></param>
/// <typeparam name="TBody">body type</typeparam>
public sealed record PulledMessage<TBody>(
        TBody Body,
        Properties Properties,
        ReceiveProperties ReceiveProperties,
        CancellationToken Cancellation,
        TaskCompletionSource<Acknowledge> Completion
    ) : Message<TBody>(Body, Properties)
{
    /// <summary>
    /// Ack received message
    /// </summary>
    /// <returns>true if success, false if message already acknowledged</returns>
    public bool Ack() => Completion.TrySetResult(Acknowledge.Ack);

    /// <summary>
    /// Nack received message
    /// </summary>
    /// <returns>true if success, false if message already acknowledged</returns>
    public bool Nack() => Completion.TrySetResult(Acknowledge.Nack);

    /// <summary>
    /// Requeue received message
    /// </summary>
    /// <returns>true if success, false if message already acknowledged</returns>
    public bool Requeue() => Completion.TrySetResult(Acknowledge.Requeue);

    public Acknowledge? AcknowledgeState
        => Completion.Task.IsCompleted ? Completion.Task.Result : null;
}
