namespace RabbitRelink;

/// <summary>
/// Topology configuration builder
/// </summary>
public interface ITopologyBuilder : ITopologyHandlerBuilder
{
    /// <summary>
    /// Configure <see cref="TopologyConfig"/> properties
    /// </summary>
    /// <param name="configure">configuration function</param>
    /// <returns><see cref="ITopologyHandlerBuilder"/></returns>
    ITopologyHandlerBuilder Configure(Apply<TopologyConfig> configure);
}
