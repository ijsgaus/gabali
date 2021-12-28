using JetBrains.Annotations;
using RabbitRelink.Connection;
using RabbitRelink.Consumer;

namespace RabbitRelink;

/// <summary>
/// Abstract base pull and push consumer common parameters
/// </summary>
public abstract record ConsumerConfigBase()
{
    /// <summary>
    /// Constant for specify <see href="https://www.rabbitmq.com/consumer-prefetch.html">prefetch</see> all queue from server
    /// </summary>
    [PublicAPI]
    public const ushort PREFETCH_ALL = 0;


    /// <summary>
    /// Consumer after error recovery interval, default 10s
    /// </summary>
    public TimeSpan RecoveryInterval { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Consumer <see href="https://www.rabbitmq.com/consumer-prefetch.html">prefetch</see> count, default <see cref="PREFETCH_ALL"/>
    /// </summary>
    public ushort PrefetchCount { get; init; } = PREFETCH_ALL;

    /// <summary>
    /// Consumer <see href="https://www.rabbitmq.com/confirms.html#acknowledgement-modes">AutoAck</see> parameter, default false
    /// </summary>
    public bool AutoAck { get; init; }

    /// <summary>
    /// Consumer <see href="https://www.rabbitmq.com/consumer-priority.html">priority</see>
    /// </summary>
    public int? Priority { get; init; }

    /// <summary>
    /// Consumer behaviour on <see href="https://www.rabbitmq.com/ha.html#cancellation">cluster problem</see>
    /// </summary>
    public bool CancelOnHaFailover { get; init; } = false;

    /// <summary>
    /// <see href="https://www.rabbitmq.com/consumers.html#exclusivity">Consumer exclusivity</see>, default false
    /// </summary>
    public bool Exclusive { get; init; } = false;

    /// <summary>
    /// Consumer <see href="https://www.rabbitmq.com/consumers.html#exclusivity">single activity</see>, default false
    /// </summary>
    // TODO:  must be realized <href>https://github.com/ijsgaus/rabbit-relink/issues/15</href>
    public bool SingleActiveConsumer { get; init; } = false;

    /// <summary>
    /// TODO: replace to observable property, <href>https://github.com/ijsgaus/rabbit-relink/issues/16</href>
    /// </summary>
    public StateHandler<RelinkConsumerState> StateChanged { get; init; } = (_, _) =>  { };

    /// <summary>
    /// TODO: replace to observable property, <href>https://github.com/ijsgaus/rabbit-relink/issues/16</href>
    /// </summary>
    public StateHandler<RelinkChannelState> ChannelStateChanged { get; init; } = (_, _) => { };
}
