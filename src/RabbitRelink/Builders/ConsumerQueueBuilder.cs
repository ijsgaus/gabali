using System.Collections.Immutable;
using RabbitRelink.Consumer;
using RabbitRelink.Middlewares;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ConsumerQueueBuilder : IConsumerQueueBuilder
{
    private readonly Relink _relink;
    private readonly IImmutableList<ConsumerMiddleware<byte[], byte[]>> _middlewares;

    public ConsumerQueueBuilder(Relink relink, IImmutableList<ConsumerMiddleware<byte[], byte[]>> middlewares)
    {
        _relink = relink;
        _middlewares = middlewares;
    }

    public IConsumerConfigBuilder<byte[]> Queue(Func<ITopologyCommander, Task<IQueue>> topologyHandler)
        => new ConsumerConfigBuilder<byte[]>((cfg, handler) =>
        {
            foreach (var middleware in _middlewares.Reverse())
                handler = middleware(handler);
            return new RelinkConsumer(cfg, _relink.CreateChannel(cfg.ChannelStateChanged, cfg.RecoveryInterval),
                topologyHandler, handler);
        });
}
