using RabbitRelink.Topology;

namespace RabbitRelink;

/// <summary>
/// Topology handler builder
/// </summary>
public interface ITopologyHandlerBuilder
{
    /// <summary>
    /// Register function for making topology settings
    /// </summary>
    /// <param name="handler">function <see cref="ITopologyCommander"/> for available topology operation</param>
    /// <returns>awaitable task, end of topology changes if success</returns>
    IRelinkTopology Handler(Func<ITopologyCommander, Task> handler);
}
