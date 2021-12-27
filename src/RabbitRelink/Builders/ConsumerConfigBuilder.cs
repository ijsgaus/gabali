using RabbitRelink.Consumer;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ConsumerConfigBuilder : IConsumerConfigBuilder
{
    private readonly Relink _relink;
    private readonly Func<ITopologyCommander, Task<IQueue>> _topologyHandler;

    public ConsumerConfigBuilder(Relink relink, Func<ITopologyCommander, Task<IQueue>> topologyHandler)
    {
        _relink = relink;
        _topologyHandler = topologyHandler;
    }

    public IRelinkPullConsumer Pull(Func<PullConsumerConfig, PullConsumerConfig>? configure = null)
    {
        configure ??= (p => p);
        var config = configure(new PullConsumerConfig());
        return new RelinkPullConsumer(config, _relink.CreateChannel(config.ChannelStateChanged, config.RecoveryInterval),
            _topologyHandler);
    }

    public IConsumerHandlerBuilder Push(Func<PushConsumerConfig, PushConsumerConfig>? configure = null)
    {
        configure ??= (p => p);
        var config = configure(new PushConsumerConfig());
        return new ConsumerHandlerBuilder(_relink, _topologyHandler, config);
    }
}
