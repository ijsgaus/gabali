using RabbitRelink.Consumer;
using RabbitRelink.Messaging;
using RabbitRelink.Middlewares;

namespace RabbitRelink;

public interface IConsumerHandlerBuilder<T> where T: class?
{
    IConsumerHandlerBuilder<TOut> Middleware<TOut>(ConsumerMiddleware<TOut, T> middleware) where TOut : class?;
    IRelinkConsumer Handler(DoConsume<T> handler);
}
