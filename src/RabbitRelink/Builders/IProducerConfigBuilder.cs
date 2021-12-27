using RabbitRelink.Middlewares;
using RabbitRelink.Producer;

namespace RabbitRelink;

public interface IProducerConfigBuilder
{
    IProducerMiddlewareBuilder<byte[]> Configure(Func<ProducerConfig, ProducerConfig> configure);

    IProducerMiddlewareBuilder<TOut> Middleware<TOut>(ProducerMiddleware<byte[], TOut> middleware) where TOut : class;
    IRelinkProducer<byte[]> Build();
}
