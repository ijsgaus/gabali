using System.Collections.Immutable;
using RabbitRelink.Middlewares;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ProducerExchangeBuilder : IProducerExchangeBuilder
{
    private readonly Relink _relink;
    private readonly IImmutableList<ProducerMiddleware<byte[], byte[]>> _middlewares;

    public ProducerExchangeBuilder(Relink relink, IImmutableList<ProducerMiddleware<byte[], byte[]>> middlewares)
    {
        _relink = relink;
        _middlewares = middlewares;
    }

    public IProducerConfigBuilder Exchange(Func<ITopologyCommander, Task<IExchange>> topologyConfig)
        => new ProducerConfigBuilder(_relink, topologyConfig, _middlewares);
}
