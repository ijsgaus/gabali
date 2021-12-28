namespace RabbitRelink.Middlewares;

public interface IMiddleware<T> : IConsumerMiddleware<T>, IProducerMiddleware<T>
{
}
