using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ProducerExchangeBuilder : IProducerExchangeBuilder
{
    private readonly Relink _relink;

    public ProducerExchangeBuilder(Relink relink)
    {
        _relink = relink;
    }

    public IProducerConfigBuilder Exchange(Func<ITopologyCommander, Task<IExchange>> topologyConfig)
        => new ProducerConfigBuilder(_relink, topologyConfig);
}
