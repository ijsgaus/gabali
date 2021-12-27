using RabbitRelink.Messaging;
using RabbitRelink.Middlewares;

namespace RabbitRelink.Producer;

internal class ProducerWrapper<TIn, TOut>  : IRelinkProducer<TOut>
    where TOut : class
    where TIn : class
{
    private readonly IRelinkProducer<TIn> _inner;
    private readonly ProducerMiddleware<TIn, TOut> _middleware;

    public ProducerWrapper(IRelinkProducer<TIn> inner, ProducerMiddleware<TIn, TOut> middleware)
    {
        _inner = inner;
        _middleware = middleware;
    }

    public void Dispose() => _inner.Dispose();

    public Guid Id => _inner.Id;
    public ProducerConfig Config => _inner.Config;
    public RelinkProducerState State => _inner.State;

    public Task WaitReadyAsync(CancellationToken cancellation = default)
        => _inner.WaitReadyAsync(cancellation);

    public Task PublishAsync(TOut body, Func<MessageProperties, MessageProperties>? configureProperties = null,
        Func<PublishProperties, PublishProperties>? configurePublish = null,
        CancellationToken cancellation = default)
        => _middleware(_inner.PublishAsync)(body, configureProperties, configurePublish, cancellation);
}
