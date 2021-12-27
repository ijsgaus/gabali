using System.Collections.Immutable;
using RabbitRelink.Consumer;
using RabbitRelink.Middlewares;
using RabbitRelink.Topology;

namespace RabbitRelink;

internal class ConsumerConfigBuilder<TIn> : IConsumerConfigBuilder<TIn> where TIn : class?
{
    private readonly Func<PushConsumerConfig, ConsumerHandler<TIn>, IRelinkConsumer> _factory;


    public ConsumerConfigBuilder(Func<PushConsumerConfig, ConsumerHandler<TIn>, IRelinkConsumer> factory)
        => _factory = factory;

    public IRelinkPullConsumer<TIn> Pull(Func<PullConsumerConfig, PullConsumerConfig>? configure = null)
    {
        configure ??= (p => p);
        var config = configure(new PullConsumerConfig());
        return new RelinkPullConsumer<TIn>(config, _factory);
    }

    public IConsumerConfigBuilder<TOut> Middleware<TOut>(ConsumerMiddleware<TOut, TIn> middleware) where TOut : class?
        => new ConsumerConfigBuilder<TOut>((cfg, handler) => _factory(cfg, middleware(handler)));

    public IConsumerHandlerBuilder<TIn> Push(Func<PushConsumerConfig, PushConsumerConfig>? configure = null)
    {
        configure ??= (p => p);
        var config = configure(new PushConsumerConfig());
        return new ConsumerHandlerBuilder<TIn>(handler => _factory(config, handler));
    }
}
