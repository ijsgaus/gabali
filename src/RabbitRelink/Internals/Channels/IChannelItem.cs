#region Usings

#endregion

namespace RabbitRelink.Internals.Channels
{
    interface IChannelItem
    {
        #region Properties

        CancellationToken Cancellation { get; }

        #endregion

        bool TrySetException(Exception ex);
        bool TrySetCanceled(CancellationToken cancellation);
    }
}
