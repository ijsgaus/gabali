#region Usings

#endregion

namespace RabbitRelink.Topology
{
    /// <summary>
    /// Represents RabbitMQ topology configurator
    /// </summary>
    public interface IRelinkTopology : IDisposable
    {
        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Operational state
        /// </summary>
        LinkTopologyState State { get; }

        /// <summary>
        /// Waits for topology ready
        /// </summary>
        Task WaitReadyAsync(CancellationToken? cancellation = null);
    }
}
