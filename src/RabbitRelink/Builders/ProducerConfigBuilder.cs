using System.Collections.Immutable;
using RabbitRelink.Middlewares;
using RabbitRelink.Producer;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ProducerConfigBuilder : IProducerConfigBuilder
{
    private readonly Relink _relink;
    private readonly Func<ITopologyCommander, Task<IExchange>> _topologyConfig;
    private readonly IImmutableList<ProducerMiddleware<byte[], byte[]>> _middlewares;

    public ProducerConfigBuilder(Relink relink, Func<ITopologyCommander, Task<IExchange>> topologyConfig,
        IImmutableList<ProducerMiddleware<byte[], byte[]>> middlewares)
    {
        _relink = relink;
        _topologyConfig = topologyConfig;
        _middlewares = middlewares;
    }

    public IProducerMiddlewareBuilder<TOut> Middleware<TOut>(ProducerMiddleware<byte[], TOut> middleware)
        where TOut : class
        => Configure(p => p).Middleware(middleware);

    public IProducerMiddlewareBuilder<byte[]> Configure(Func<ProducerConfig, ProducerConfig> configure)
    {
        var config = configure(new ProducerConfig());
        return new ProducerMiddlewareBuilder<byte[]>(() =>
        {
            IRelinkProducer<byte[]> inner = new RelinkProducer(
                _relink.CreateChannel(config.OnChannelStateChanges, config.RecoveryInterval),
                config, _topologyConfig);
            foreach (var middleware in _middlewares.Reverse())
            {
                inner = new ProducerWrapper<byte[], byte[]>(inner, middleware);
            }

            return inner;
        });
    }

    public IRelinkProducer<byte[]> Build() => Configure(p => p).Build();

}
