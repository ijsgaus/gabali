using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ConsumerQueueBuilder : IConsumerQueueBuilder
{
    private readonly Relink _relink;

    public ConsumerQueueBuilder(Relink relink)
    {
        _relink = relink;
    }

    public IConsumerConfigBuilder Queue(Func<ITopologyCommander, Task<IQueue>> topologyHandler)
        => new ConsumerConfigBuilder(_relink, topologyHandler);
}
