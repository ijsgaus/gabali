#region Usings

using RabbitRelink.Internals.Channels;

#endregion

namespace RabbitRelink.Internals.Actions
{
    internal class ActionItem<TActor> : ChannelItem<Func<TActor, object>, object>
    {
        #region Ctor

        public ActionItem(Func<TActor, object> value, CancellationToken cancellationToken) : base(value,
            cancellationToken)
        {
        }

        #endregion
    }
}
