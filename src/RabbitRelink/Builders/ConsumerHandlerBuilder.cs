using RabbitRelink.Consumer;
using RabbitRelink.Messaging;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ConsumerHandlerBuilder : IConsumerHandlerBuilder
{
    private readonly Relink _relink;
    private readonly Func<ITopologyCommander, Task<IQueue>> _topologyHandler;
    private readonly PushConsumerConfig _config;

    public ConsumerHandlerBuilder(Relink relink, Func<ITopologyCommander, Task<IQueue>> topologyHandler, PushConsumerConfig config)
    {
        _relink = relink;
        _topologyHandler = topologyHandler;
        _config = config;
    }

    public IRelinkConsumer Handler(Func<ConsumedMessage<byte[]>, Task<Acknowledge>> handler)
        => new RelinkConsumer(_config, _relink.CreateChannel(_config.ChannelStateChanged, _config.RecoveryInterval),
            _topologyHandler, handler);
}
