#region Usings

#endregion

using RabbitRelink.Internals.Channels;

namespace RabbitRelink.Consumer
{
    internal class RelinkConsumerMessageAction : ChannelItem
    {
        #region Ctor

        public RelinkConsumerMessageAction(
            ulong seq,
            Acknowledge strategy,
            CancellationToken cancellation
        ) : base(cancellation)
        {
            Seq = seq;
            Strategy = strategy;
        }

        #endregion

        #region Properties

        public ulong Seq { get; }
        public Acknowledge Strategy { get; }

        #endregion
    }
}
