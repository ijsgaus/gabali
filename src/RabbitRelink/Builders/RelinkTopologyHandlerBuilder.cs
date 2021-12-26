using RabbitRelink.Topology;
using RabbitRelink.Topology.Internal;

namespace RabbitRelink;

internal class RelinkTopologyHandlerBuilder : IRelinkTopologyHandlerBuilder
{
    protected readonly Relink Relink;
    private Func<RelinkTopologyConfig, RelinkTopologyConfig>? _configure;

    public RelinkTopologyHandlerBuilder(Relink relink, Func<RelinkTopologyConfig, RelinkTopologyConfig>? configure)
    {
        Relink = relink;
        _configure = configure;
    }

    public IRelinkTopology Handler(Func<ITopologyCommander, Task> handler)
    {
        _configure ??= (p => p);
        var config = _configure(new RelinkTopologyConfig());
        return new RelinkTopology(
            Relink.CreateChannel(config.OnChannelStateChanged, config.RecoveryInterval),
            config,
            handler);
    }
}
