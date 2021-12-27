using RabbitRelink.Producer;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ProducerConfigBuilder : IProducerConfigBuilder
{
    private readonly Relink _relink;
    private readonly Func<ITopologyCommander, Task<IExchange>> _topologyConfig;

    public ProducerConfigBuilder(Relink relink, Func<ITopologyCommander, Task<IExchange>> topologyConfig)
    {
        _relink = relink;
        _topologyConfig = topologyConfig;
    }

    public IRelinkProducer Configure(Func<ProducerConfig, ProducerConfig> configure)
    {
        var config = configure(new ProducerConfig());
        return new RelinkProducer(_relink.CreateChannel(config.OnChannelStateChanges, config.RecoveryInterval),
            config, _topologyConfig);
    }

    public IRelinkProducer Build() => Configure(p => p);

}
