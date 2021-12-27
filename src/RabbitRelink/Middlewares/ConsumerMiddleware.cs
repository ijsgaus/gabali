namespace RabbitRelink.Middlewares;

public delegate ConsumerHandler<TOut> ConsumerMiddleware<TIn, TOut>(ConsumerHandler<TIn> next)
    where TIn : class? where TOut : class?;
