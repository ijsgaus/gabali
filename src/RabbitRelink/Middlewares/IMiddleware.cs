namespace RabbitRelink.Middlewares;

public interface IMiddleware : IConsumerMiddleware<byte[], byte[]>, IProducerMiddleware<byte[], byte[]>
{
}
