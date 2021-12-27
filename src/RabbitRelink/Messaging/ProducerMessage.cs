#region Usings

#endregion

using RabbitRelink.Internals.Channels;

namespace RabbitRelink.Messaging
{
    public record ProducerMessage<T>(T Body, MessageProperties Properties, PublishProperties PublishProperties)
        : Message<T>(Body, Properties), IChannelItem where T : class
    {
        private readonly TaskCompletionSource<object?> _tcs =
            new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        internal bool TrySetResult() => _tcs.TrySetResult(null);

        internal bool TrySetException(Exception ex) => _tcs.TrySetException(ex);

        internal bool TrySetCanceled(CancellationToken cancellation) => _tcs.TrySetCanceled(cancellation);

        internal Task Completion => _tcs.Task;

        internal CancellationToken Cancellation { get; set; }



        CancellationToken IChannelItem.Cancellation => Cancellation;

        bool IChannelItem.TrySetException(Exception ex) => TrySetException(ex);

        bool IChannelItem.TrySetCanceled(CancellationToken cancellation) => TrySetCanceled(cancellation);
    }
}
