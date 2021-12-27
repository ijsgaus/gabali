namespace RabbitRelink;

public record PushConsumerConfig() : ConsumerConfigBase()
{
    public const ushort PARALLELISM_FULL = 0;
    public ushort Parallelism { get; init; } = PARALLELISM_FULL;
}
