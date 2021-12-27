using System.Collections.Immutable;
using RabbitRelink.Middlewares;

namespace RabbitRelink;

internal record RelinkBuilder(Uri Uri) : IRelinkBuilder
{
    public RelinkConfig Config { get; init; } = new RelinkConfig();
    public IImmutableSet<string> Hosts { get; init; } = ImmutableHashSet<string>.Empty;
    public IImmutableList<ConsumerMiddleware<byte[], byte[]>> ConsumerMiddlewares { get; init; } = ImmutableList<ConsumerMiddleware<byte[], byte[]>>.Empty;
    public IImmutableList<ProducerMiddleware<byte[], byte[]>> ProducerMiddlewares { get; init; } = ImmutableList<ProducerMiddleware<byte[], byte[]>>.Empty;

    IRelinkBuilder IRelinkBuilder.Hosts(params string[] hosts)
        => this with {Hosts = Hosts.Union(hosts.ToImmutableHashSet())};

    IRelinkBuilder IRelinkBuilder.Configure(Func<RelinkConfig, RelinkConfig> configure)
        => this with {Config = configure(Config)};

    IRelinkBuilder IRelinkBuilder.Middleware(ConsumerMiddleware<byte[], byte[]> middleware,
        params ConsumerMiddleware<byte[], byte[]>[] middlewares)
        => this with {ConsumerMiddlewares = ConsumerMiddlewares.Add(middleware).AddRange(middlewares)};

    IRelinkBuilder IRelinkBuilder.Middleware(ProducerMiddleware<byte[], byte[]> middleware,
        params ProducerMiddleware<byte[], byte[]>[] middlewares)
        => this with {ProducerMiddlewares = ProducerMiddlewares.Add(middleware).AddRange(middlewares)};

    IRelink IRelinkBuilder.Build()
        => new Relink(Uri, Config, Hosts, ProducerMiddlewares, ConsumerMiddlewares);
}
