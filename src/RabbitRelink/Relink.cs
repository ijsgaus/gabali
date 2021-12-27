using System.Collections.Immutable;
using System.Dynamic;
using RabbitRelink.Connection;
using RabbitRelink.Messaging;
using RabbitRelink.Middlewares;
using RabbitRelink.Topology;
using RabbitRelink.Topology.Internal;

namespace RabbitRelink;

public class Relink : IRelink
{
    private readonly IImmutableList<ProducerMiddleware<byte[], byte[]>> _producerMiddlewares;
    private readonly IImmutableList<ConsumerMiddleware<byte[], byte[]>> _consumerMiddlewares;

    private readonly IRelinkConnection _connection;
    public Uri Uri => _connection.Uri;
    public IImmutableSet<string> Hosts => _connection.Hosts;
    public RelinkConfig Config => _connection.Config;


    public static IRelinkBuilder Create(Uri uri)
        => new RelinkBuilder(uri);

    public static IRelinkBuilder Create(string uri)
        => Create(new Uri(uri));


    internal Relink(Uri uri, RelinkConfig config, IImmutableSet<string> hosts,
        IImmutableList<ProducerMiddleware<byte[], byte[]>> producerMiddlewares,
        IImmutableList<ConsumerMiddleware<byte[], byte[]>>  consumerMiddlewares)
    {
        _producerMiddlewares = producerMiddlewares;
        _consumerMiddlewares = consumerMiddlewares;
        _connection = new RelinkConnection(uri, config, hosts.Add(uri.Host));
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

    public ITopologyBuilder Topology() => new TopologyBuilder(this);
    public IConsumerQueueBuilder Consumer() => new ConsumerQueueBuilder(this, _consumerMiddlewares);

    public IProducerExchangeBuilder Producer() => new ProducerExchangeBuilder(this, _producerMiddlewares);

}
