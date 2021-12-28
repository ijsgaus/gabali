using RabbitRelink.Middlewares;

namespace RabbitRelink;

/// <summary>
/// Configure <see cref="IRelink"/> for using
/// </summary>
public interface IRelinkBuilder
{
    /// <summary>
    /// Add hosts to cluster configuration
    /// </summary>
    /// <param name="hosts">host list</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Hosts(params string[] hosts);

    /// <summary>
    /// Configure <see cref="RelinkConfig"/> properties
    /// </summary>
    /// <param name="configure">configuration function</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Configure(Apply<RelinkConfig> configure);

    /// <summary>
    /// Add consumer middleware/middlewares
    /// </summary>
    /// <param name="middleware">single mandatory middleware</param>
    /// <param name="middlewares">other middlewares</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Middleware(ConsumerMiddleware<byte[]> middleware,
        params ConsumerMiddleware<byte[]>[] middlewares);

    /// <summary>
    /// Add producer middleware/middlewares
    /// </summary>
    /// <param name="middleware">single mandatory middleware</param>
    /// <param name="middlewares">other middlewares</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Middleware(ProducerMiddleware<byte[]> middleware,
        params ProducerMiddleware<byte[]>[] middlewares);

    /// <summary>
    /// Add symmetric middleware
    /// </summary>
    /// <param name="middleware">single mandatory middleware</param>
    /// <param name="middlewares">other middlewares</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Middleware(IMiddleware<byte[]> middleware, params IMiddleware<byte[]>[] middlewares)
        => Middleware(middleware.AsConsumerMiddleware(),
                middlewares
                    .Select(p => p.AsConsumerMiddleware())
                    .ToArray())
            .Middleware(middleware.AsProducerMiddleware(),
                middlewares
                    .Select(p => p.AsProducerMiddleware())
                    .ToArray());

    /// <summary>
    /// Add producer middleware/middlewares as interface
    /// </summary>
    /// <param name="middleware">single mandatory middleware</param>
    /// <param name="middlewares">other middlewares</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Middleware(IProducerMiddleware<byte[]> middleware,
        params IProducerMiddleware<byte[]>[] middlewares)
        => Middleware(middleware.AsProducerMiddleware(),
            middlewares
                .Select(p => p.AsProducerMiddleware())
                .ToArray());

    /// <summary>
    /// Add consumer middleware/middlewares as interface
    /// </summary>
    /// <param name="middleware">single mandatory middleware</param>
    /// <param name="middlewares">other middlewares</param>
    /// <returns><see cref="IRelinkBuilder"/></returns>
    IRelinkBuilder Middleware(IConsumerMiddleware<byte[]> middleware,
        params IConsumerMiddleware<byte[]>[] middlewares)
        => Middleware(middleware.AsConsumerMiddleware(),
            middlewares
                .Select(p => p.AsConsumerMiddleware())
                .ToArray());

    /// <summary>
    /// Build <see cref="IRelink"/>
    /// </summary>
    /// <returns><see cref="IRelink"/></returns>
    IRelink Build();
}
