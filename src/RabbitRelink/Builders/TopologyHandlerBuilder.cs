using RabbitRelink.Topology;
using RabbitRelink.Topology.Internal;

namespace RabbitRelink;

internal class TopologyHandlerBuilder : ITopologyHandlerBuilder
{
    protected readonly Relink Relink;
    private Apply<TopologyConfig>? _configure;

    internal TopologyHandlerBuilder(Relink relink, Apply<TopologyConfig>? configure)
    {
        Relink = relink;
        _configure = configure;
    }

    public IRelinkTopology Handler(Func<ITopologyCommander, Task> handler)
    {
        var config = (_configure ?? Fn.Id)(new TopologyConfig());
        return new RelinkTopology(
            Relink.CreateChannel(config.OnChannelStateChanged, config.RecoveryInterval),
            config,
            handler);
    }
}
