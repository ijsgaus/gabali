using RabbitRelink.Topology;
using RabbitRelink.Topology.Internal;

namespace RabbitRelink;

internal class TopologyHandlerBuilder : ITopologyHandlerBuilder
{
    protected readonly Relink Relink;
    private Func<TopologyConfig, TopologyConfig>? _configure;

    public TopologyHandlerBuilder(Relink relink, Func<TopologyConfig, TopologyConfig>? configure)
    {
        Relink = relink;
        _configure = configure;
    }

    public IRelinkTopology Handler(Func<ITopologyCommander, Task> handler)
    {
        _configure ??= (p => p);
        var config = _configure(new TopologyConfig());
        return new RelinkTopology(
            Relink.CreateChannel(config.OnChannelStateChanged, config.RecoveryInterval),
            config,
            handler);
    }
}
