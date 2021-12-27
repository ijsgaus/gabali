namespace RabbitRelink.Middlewares;

public interface IProducerMiddleware<TIn, TOut>
where TIn: class?
where TOut: class?
{
    PublishMessage<TOut> NextProducer(PublishMessage<TIn> next);
}
