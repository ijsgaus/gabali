namespace RabbitRelink;

public interface ITopologyBuilder : ITopologyHandlerBuilder
{
    ITopologyHandlerBuilder Configure(Func<TopologyConfig, TopologyConfig> configure);
}
