using RabbitRelink.Consumer;
using RabbitRelink.Messaging;

namespace RabbitRelink.Middlewares;

public interface IConsumerMiddleware<TIn, TOut>
    where TOut : class?
    where TIn: class?
{
    ConsumerHandler<TOut> NextConsumer(ConsumerHandler<TIn> next);
}


