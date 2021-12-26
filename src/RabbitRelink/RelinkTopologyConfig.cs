using RabbitRelink.Connection;
using RabbitRelink.Topology;

namespace RabbitRelink;

/// <summary>
/// Relink topology configuration
/// </summary>
public record RelinkTopologyConfig()
{
    /// <summary>
    /// Retry topology configuration interval (default 10s)
    /// </summary>
    public TimeSpan RecoveryInterval { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Change topology state handler (default noop)
    /// </summary>
    public StateHandler<LinkTopologyState> OnStateChanged { get; init; } = (_, _) => { };

    /// <summary>
    /// Change topology channel state handler (default noop)
    /// </summary>
    public StateHandler<RelinkChannelState> OnChannelStateChanged { get; init; }  = (_, _) => { };
}


