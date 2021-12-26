namespace RabbitRelink.Topology.Internal
{
    internal interface IRelinkTopologyInternal : IRelinkTopology
    {
        event EventHandler? Disposed;
    }
}
