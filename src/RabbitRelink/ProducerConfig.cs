using RabbitRelink.Connection;
using RabbitRelink.Messaging;
using RabbitRelink.Producer;

namespace RabbitRelink;

public record ProducerConfig()
{
    public bool ConfirmsMode { get; init; } = true;
    public Func<Properties, Properties> UpdateProperties { get; init; } = (p => p);
    public Func<PublishProperties, PublishProperties> UpdatePublish { get; init; } = (p => p);
    public TimeSpan RecoveryInterval { get; init; } = TimeSpan.FromSeconds(10);
    public IMessageIdGenerator IdGenerator { get; init; } = new GuidMessageIdGenerator();
    public StateHandler<RelinkProducerState> OnStateChanged { get; init; } = (_, _) => { };
    public StateHandler<RelinkChannelState> OnChannelStateChanges { get; init; } = (_, _) => { };
}
