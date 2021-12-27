namespace RabbitRelink.Middlewares;

public delegate PublishMessage<TOut> ProducerMiddleware<TIn, TOut>(PublishMessage<TIn> next);
