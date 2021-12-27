namespace RabbitRelink;

public record PullConsumerConfig() : ConsumerConfigBase()
{
    public const ushort UNBOUND = 0;
    public ushort BoundChannel { get; init; } = UNBOUND;
}
