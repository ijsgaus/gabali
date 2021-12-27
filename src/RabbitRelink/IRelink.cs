using System.Collections.Immutable;
using RabbitRelink.Topology;

namespace RabbitRelink;

public interface IRelink : IDisposable
{
    Uri Uri { get; }
    IImmutableSet<string> Hosts { get; }
    RelinkConfig Config { get; }

    /// <summary>
    ///     Is Link connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///     Invokes when connected, must not perform blocking operations.
    /// </summary>
    event EventHandler Connected;

    /// <summary>
    ///     Invokes when disconnected, must not perform blocking operations.
    /// </summary>
    event EventHandler Disconnected;

    /// <summary>
    ///     Initializes connection
    /// </summary>
    void Initialize();

    ITopologyBuilder Topology();
    IConsumerQueueBuilder Consumer();
}
