using RabbitRelink.Topology;

namespace RabbitRelink;

public interface IProducerExchangeBuilder
{
    IProducerConfigBuilder Exchange(Func<ITopologyCommander, Task<IExchange>> topologyConfig);
}
