using System.Collections.Immutable;
using RabbitRelink.Connection;
using RabbitRelink.Topology;
using RabbitRelink.Topology.Internal;

namespace RabbitRelink;

public class Relink : IRelink
{
    private readonly IRelinkConnection _connection;
    public Uri Uri => _connection.Uri;
    public IImmutableSet<string> Hosts => _connection.Hosts;
    public RelinkConfig Config => _connection.Config;

    public static IRelink Create(string uri, Func<RelinkConfig, RelinkConfig>? configure = null,
        params string[] hostNames)
        => new Relink(new Uri(uri), configure, hostNames);

    public static IRelink Create(Uri uri, Func<RelinkConfig, RelinkConfig>? configure = null, params string[] hostNames)
        => new Relink(uri, configure, hostNames);

    private Relink(Uri uri, Func<RelinkConfig, RelinkConfig>? configure = null, params string[] hostNames)
    {
        var hosts = hostNames.Append(uri.Host).ToImmutableHashSet();
        configure = configure ?? (p => p);
        var config = configure(new RelinkConfig());
        _connection = new RelinkConnection(uri, config, hosts);
    }

    public void Dispose() => _connection.Dispose();


    public bool IsConnected => _connection.State == RelinkConnectionState.Active;

    public event EventHandler? Connected
    {
        add => _connection.Connected += value;
        remove => _connection.Connected -= value;
    }

    public event EventHandler? Disconnected
    {
        add => _connection.Disconnected += value;
        remove => _connection.Disconnected -= value;
    }

    public void Initialize() => _connection.Initialize();

    internal IRelinkChannel CreateChannel(StateHandler<RelinkChannelState> stateHandler, TimeSpan recoveryInterval)
        => new RelinkChannel(_connection, stateHandler, recoveryInterval);

    public IRelinkTopologyBuilder Topology() => new RelinkTopologyBuilder(this);

}
