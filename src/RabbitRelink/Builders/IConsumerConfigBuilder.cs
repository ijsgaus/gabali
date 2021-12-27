using RabbitRelink.Consumer;
using RabbitRelink.Middlewares;

namespace RabbitRelink;

public interface IConsumerConfigBuilder<TIn> where TIn: class
{
    IConsumerConfigBuilder<TOut> Middleware<TOut>(ConsumerMiddleware<TOut, TIn> middleware) where TOut : class;
    IRelinkPullConsumer<TIn> Pull(Func<PullConsumerConfig, PullConsumerConfig>? configure = null);
    IConsumerHandlerBuilder<TIn> Push(Func<PushConsumerConfig, PushConsumerConfig>? configure = null);
}
