namespace RabbitRelink.Middlewares;

public delegate PublishMessage<TOut> PublishMiddleware<TIn, TOut>(PublishMessage<TIn> next);
