using RabbitRelink.Middlewares;

namespace RabbitRelink;
public interface IRelinkBuilder
{
    IRelinkBuilder Hosts(params string[] hosts);
    IRelinkBuilder Configure(Func<RelinkConfig, RelinkConfig> configure);

    IRelinkBuilder Middleware(ConsumerMiddleware<byte[]?, byte[]?> middleware,
        params ConsumerMiddleware<byte[]?, byte[]?>[] middlewares);

    IRelinkBuilder Middleware(ProducerMiddleware<byte[]?, byte[]?> middleware,
        params ProducerMiddleware<byte[]?, byte[]?>[] middlewares);

    IRelinkBuilder Middleware(IMiddleware middleware, params IMiddleware[] middlewares)
        => Middleware(middleware.NextConsumer,
                middlewares
                    .Select<IConsumerMiddleware<byte[]?, byte[]?>, ConsumerMiddleware<byte[]?, byte[]?>>(p => p.NextConsumer)
                    .ToArray())
            .Middleware(middleware.NextProducer,
                middlewares
                    .Select<IProducerMiddleware<byte[]?, byte[]?>, ProducerMiddleware<byte[]?, byte[]?>>(p => p.NextProducer)
                    .ToArray());

    IRelinkBuilder Middleware(IProducerMiddleware<byte[]?, byte[]?> middleware,
        params IProducerMiddleware<byte[]?, byte[]?>[] middlewares)
        => Middleware(middleware.NextProducer,
            middlewares
                .Select<IProducerMiddleware<byte[]?, byte[]?>, ProducerMiddleware<byte[]?, byte[]?>>(p => p.NextProducer)
                .ToArray());

    IRelinkBuilder Middleware(IConsumerMiddleware<byte[]?, byte[]?> middleware,
        params IConsumerMiddleware<byte[]?, byte[]?>[] middlewares)
        => Middleware(middleware.NextConsumer,
            middlewares
                .Select<IConsumerMiddleware<byte[]?, byte[]?>, ConsumerMiddleware<byte[]?, byte[]?>>(p => p.NextConsumer)
                .ToArray());
    IRelink Build();
}
