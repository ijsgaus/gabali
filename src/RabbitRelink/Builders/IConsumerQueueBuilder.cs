using RabbitRelink.Topology;

namespace RabbitRelink;

public interface IConsumerQueueBuilder
{
    IConsumerConfigBuilder Queue(Func<ITopologyCommander, Task<IQueue>> topologyHandler);
}
