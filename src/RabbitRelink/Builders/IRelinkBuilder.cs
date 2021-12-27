using RabbitRelink.Middlewares;

namespace RabbitRelink;
public interface IRelinkBuilder
{
    IRelinkBuilder Hosts(params string[] hosts);
    IRelinkBuilder Configure(Func<RelinkConfig, RelinkConfig> configure);

    IRelinkBuilder Middleware(ConsumerMiddleware<byte[], byte[]> middleware,
        params ConsumerMiddleware<byte[], byte[]>[] middlewares);

    IRelinkBuilder Middleware(ProducerMiddleware<byte[], byte[]> middleware,
        params ProducerMiddleware<byte[], byte[]>[] middlewares);

    IRelinkBuilder Middleware(IMiddleware middleware, params IMiddleware[] middlewares)
        => Middleware(middleware.NextConsumer,
                middlewares
                    .Select(p => p.NextConsumer)
                    .Cast<ConsumerMiddleware<byte[], byte[]>>()
                    .ToArray())
            .Middleware(middleware.NextProducer,
                middlewares
                    .Select(p => p.NextProducer)
                    .Cast<ProducerMiddleware<byte[], byte[]>>()
                    .ToArray());

    IRelinkBuilder Middleware(IProducerMiddleware<byte[], byte[]> middleware,
        params IProducerMiddleware<byte[], byte[]>[] middlewares)
        => Middleware(middleware.NextProducer,
            middlewares
                .Select(p => p.NextProducer)
                .Cast<ProducerMiddleware<byte[], byte[]>>()
                .ToArray());

    IRelinkBuilder Middleware(IConsumerMiddleware<byte[], byte[]> middleware,
        params IConsumerMiddleware<byte[], byte[]>[] middlewares)
        => Middleware(middleware.NextConsumer,
            middlewares
                .Select(p => p.NextConsumer)
                .Cast<ConsumerMiddleware<byte[], byte[]>>()
                .ToArray());


    IRelink Build();
}
