using RabbitRelink.Topology;

namespace RabbitRelink;

public interface ITopologyHandlerBuilder
{
    IRelinkTopology Handler(Func<ITopologyCommander, Task> handler);
}
