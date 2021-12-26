using RabbitRelink.Topology;

namespace RabbitRelink;

public interface IRelinkTopologyHandlerBuilder
{
    IRelinkTopology Handler(Func<ITopologyCommander, Task> handler);
}
