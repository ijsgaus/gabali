using RabbitRelink.Middlewares;
using RabbitRelink.Producer;

namespace RabbitRelink;

internal class ProducerMiddlewareBuilder<TIn> : IProducerMiddlewareBuilder<TIn> where TIn : class
{
    private readonly Func<IRelinkProducer<TIn>> _factory;


    public ProducerMiddlewareBuilder(Func<IRelinkProducer<TIn>> factory)
    {
        _factory = factory;
    }

    public IProducerMiddlewareBuilder<TOut> Middleware<TOut>(ProducerMiddleware<TIn, TOut> middleware)
        where TOut : class
        => new ProducerMiddlewareBuilder<TOut>(
            () => new ProducerWrapper<TIn, TOut>(_factory(), middleware)
        );

    public IRelinkProducer<TIn> Build()
        => _factory();
}
