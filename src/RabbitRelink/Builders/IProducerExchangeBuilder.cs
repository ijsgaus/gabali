using RabbitRelink.Topology;

namespace RabbitRelink;

public interface IProducerExchangeBuilder
{
    IProducerConfigBuilder Exchange(Func<ITopologyCommander, Func<IExchange>> topologyConfig);
}
