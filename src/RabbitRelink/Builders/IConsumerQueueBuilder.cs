using RabbitRelink.Topology;

namespace RabbitRelink;

public interface IConsumerQueueBuilder
{
    IConsumerConfigBuilder<byte[]> Queue(Func<ITopologyCommander, Task<IQueue>> topologyHandler);
}

