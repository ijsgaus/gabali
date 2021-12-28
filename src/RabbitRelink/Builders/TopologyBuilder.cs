namespace RabbitRelink;

internal class TopologyBuilder : TopologyHandlerBuilder, ITopologyBuilder
{
    public TopologyBuilder(Relink relink) : base(relink, null)
    {
    }

    public ITopologyHandlerBuilder Configure(Apply<TopologyConfig> configure)
    {
        return new TopologyHandlerBuilder(Relink, configure);
    }
}
