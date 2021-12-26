using System.Collections.Immutable;
using RabbitMQ.Client;

namespace RabbitRelink.Connection;

/// <summary>
///     Represents <see cref="IConnection"/> with automatic recovering
/// </summary>
internal interface IRelinkConnection : IDisposable
{
    #region Properties

    /// <summary>
    /// Identifier
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Operational state
    /// </summary>
    RelinkConnectionState State { get; }

    /// <summary>
    /// Id of user who initialize connection
    /// </summary>
    string UserId { get; }

    #endregion

    /// <summary>
    /// Emitted when disposed
    /// </summary>
    event EventHandler? Disposed;

    /// <summary>
    /// Emitted when connected
    /// </summary>
    event EventHandler? Connected;

    /// <summary>
    /// Emitted when disconnected
    /// </summary>
    event EventHandler? Disconnected;

    /// <summary>
    /// Initialize connection
    /// </summary>
    void Initialize();

    /// <summary>
    /// Create <see cref="IModel"/>
    /// </summary>
    /// <param name="cancellation">Action cancellation</param>
    Task<IModel> CreateModelAsync(CancellationToken cancellation);

    /// <summary>
    /// Configuration
    /// </summary>
    RelinkConfig Config { get; }

    Uri Uri { get; }
    IImmutableSet<string> Hosts { get; }
}
