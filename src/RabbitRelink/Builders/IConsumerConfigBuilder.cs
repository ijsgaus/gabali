using RabbitRelink.Consumer;
using RabbitRelink.Middlewares;

namespace RabbitRelink;

public interface IConsumerConfigBuilder
{
    IConsumerConfigBuilder<T> Deserializer<T>(Deserializer<T> deserializer);
    IConsumerConfigBuilder Middleware(ConsumerMiddleware<byte[]> middleware);
    IRelinkPullConsumer<byte[]> Pull(Func<PullConsumerConfig, PullConsumerConfig>? configure = null);
    IConsumerHandlerBuilder<byte[]> Push(Func<PushConsumerConfig, PushConsumerConfig>? configure = null);
}

public interface IConsumerConfigBuilder<T>
{
    IConsumerConfigBuilder<T> Middleware(ConsumerMiddleware<T> middleware)
    IRelinkPullConsumer<T> Pull(Func<PullConsumerConfig, PullConsumerConfig>? configure = null);
    IConsumerHandlerBuilder<T> Push(Func<PushConsumerConfig, PushConsumerConfig>? configure = null);
}
