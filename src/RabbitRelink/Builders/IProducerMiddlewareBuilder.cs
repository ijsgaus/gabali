using RabbitRelink.Middlewares;
using RabbitRelink.Producer;

namespace RabbitRelink;

public interface IProducerMiddlewareBuilder<TIn> where TIn : class?
{
    IProducerMiddlewareBuilder<TOut> Middleware<TOut>(ProducerMiddleware<TIn, TOut> middleware) where TOut : class?;
    IRelinkProducer<TIn> Build();
}
