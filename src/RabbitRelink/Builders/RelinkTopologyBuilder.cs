namespace RabbitRelink;

internal class RelinkTopologyBuilder : RelinkTopologyHandlerBuilder, IRelinkTopologyBuilder
{
    public RelinkTopologyBuilder(Relink relink) : base(relink, null)
    {
    }

    public IRelinkTopologyHandlerBuilder Configure(Func<RelinkTopologyConfig, RelinkTopologyConfig> configure)
    {
        return new RelinkTopologyHandlerBuilder(Relink, configure);
    }
}
