namespace RabbitRelink;

public interface IRelinkTopologyBuilder : IRelinkTopologyHandlerBuilder
{
    IRelinkTopologyHandlerBuilder Configure(Func<RelinkTopologyConfig, RelinkTopologyConfig> configure);
}
