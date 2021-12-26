#region Usings

#endregion

using RabbitMQ.Client.Events;
using RabbitRelink.Consumer;

namespace RabbitRelink.Messaging
{
    public record PulledMessage<TBody>(TBody Body, MessageProperties Properties, ReceiveProperties ReceiveProperties,
        CancellationToken Cancellation, TaskCompletionSource<Acknowledge> Completion) : ConsumedMessage<TBody>(Body, Properties, ReceiveProperties, Cancellation) where TBody : class
    {
        public void Ack() => Completion.TrySetResult(Acknowledge.Ack);

        public void Nack() => Completion.TrySetResult(Acknowledge.Nack);

        public void Requeue() => Completion.TrySetResult(Acknowledge.Requeue);


        public PulledMessage(ConsumedMessage<TBody> message, TaskCompletionSource<Acknowledge> completion)
            : this(message.Body, message.Properties, message.ReceiveProperties, message.Cancellation, completion)
        {

        }
    }
}
