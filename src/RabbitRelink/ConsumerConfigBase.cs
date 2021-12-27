using RabbitRelink.Connection;
using RabbitRelink.Consumer;

namespace RabbitRelink;

public abstract record ConsumerConfigBase()
{
    public const ushort PREFETCH_ALL = 0;


    public TimeSpan RecoveryInterval { get; init; } = TimeSpan.FromSeconds(10);
    public ushort PrefetchCount { get; init; } = PREFETCH_ALL;
    public bool AutoAck { get; init; } = false;
    public int? Priority { get; init; }
    public bool CancelOnHaFailover { get; init; } = false;
    public bool Exclusive { get; init; } = false;
    public StateHandler<RelinkConsumerState> StateChanged { get; init; } = (_, _) =>  { };
    public StateHandler<RelinkChannelState> ChannelStateChanged { get; init; } = (_, _) => { };
}
