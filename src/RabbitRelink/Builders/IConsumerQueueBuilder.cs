using RabbitRelink.Topology;

namespace RabbitRelink;

/// <summary>
/// Consumer queue topology builder
/// </summary>
public interface IConsumerQueueBuilder
{
    /// <summary>
    /// Configure topology preparing routine and consumer queue name
    /// </summary>
    /// <param name="topologyHandler">topology preparing routine</param>
    /// <returns><see cref="IConsumerConfigBuilder{TIn}"/></returns>
    IConsumerConfigBuilder<byte[]?> Queue(Func<ITopologyCommander, Task<IQueue>> topologyHandler);
}

