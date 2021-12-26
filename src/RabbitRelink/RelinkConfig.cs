using System.Reflection;
using RabbitRelink.Connection;
using RabbitRelink.Logging;

namespace RabbitRelink;

/// <summary>
/// Relink configuration options
/// </summary>
public record RelinkConfig()
{
    /// <summary>
    /// Is connection must start automatically (default true).
    /// </summary>
    public bool AutoStart { get; init; } = true;

    /// <summary>
    /// Use background threads for connection handling (default false).
    /// </summary>
    public bool UseBackgroundThreadsForConnection { get; init; } = false;

    /// <summary>
    /// Sets handler for state changes (default noop).
    /// </summary>
    public StateHandler<RelinkConnectionState> StateHandler { get; init; } = (_, _) => { };

    /// <summary>
    /// Name of connection (default - [starting assembly or exe file name]:[machine name]
    /// </summary>
    public string ConnectionName { get; init; } = $"{GetAppName()}:{Environment.MachineName}";

    /// <summary>
    /// Connection timeout (default 10 seconds).
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    ///  Timeout before next connection attempt (default 10 seconds).
    /// </summary>
    public TimeSpan RecoveryInterval { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Logger factory  (default uses <see cref="NullLoggerFactory" />
    /// </summary>
    public IRelinkLoggerFactory LoggerFactory { get; init; } = new NullLoggerFactory();

    /// <summary>
    /// Sets <see cref="LinkMessageProperties.AppId" /> to all published messages, white spaces will be trimmed, must be
    /// not null or white space (default starting assembly or exe file name)
    /// </summary>
    public string AppId { get; init; } = GetAppName();

    private static string GetAppName() => Assembly.GetEntryAssembly()?.FullName ?? Environment.GetCommandLineArgs()[0];
}
