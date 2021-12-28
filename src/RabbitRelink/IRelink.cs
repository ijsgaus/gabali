using System.Collections.Immutable;
using RabbitRelink.Topology;

namespace RabbitRelink;

/// <summary>
/// Main RabbitMq connection interface
/// </summary>
public interface IRelink : IDisposable
{
    /// <summary>
    /// Connection uri, <see href="https://www.rabbitmq.com/uri-spec.html"/>
    /// </summary>
    Uri Uri { get; }

    /// <summary>
    /// Set of host names in cluster
    /// </summary>
    IImmutableSet<string> Hosts { get; }

    /// <summary>
    /// <see cref="RelinkConfig"/> values for this instance
    /// </summary>
    RelinkConfig Config { get; }

    /// <summary>
    /// Is Relink connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Invokes when connected, must not perform blocking operations.
    /// </summary>
    event EventHandler? Connected;

    /// <summary>
    /// Invokes when disconnected, must not perform blocking operations.
    /// </summary>
    event EventHandler? Disconnected;

    /// <summary>
    /// Initializes connection, if no <see cref="RelinkConfig.AutoStart"/> specified, elsewhere nope
    /// </summary>
    void Initialize();

    /// <summary>
    /// Create instance of <see cref="ITopologyBuilder"/>
    /// </summary>
    /// <returns><see cref="ITopologyBuilder">Topology builder</see></returns>
    ITopologyBuilder Topology();

    /// <summary>
    /// Create instance of <see cref="IConsumerQueueBuilder"/>
    /// </summary>
    /// <returns><see cref="IConsumerQueueBuilder">Consumer queue builder</see></returns>
    IConsumerQueueBuilder Consumer();

    /// <summary>
    /// Create instance of <see cref="IProducerExchangeBuilder"/>
    /// </summary>
    /// <returns><see cref="IProducerExchangeBuilder">Producer exchange builder</see></returns>
    IProducerExchangeBuilder Producer();
}
