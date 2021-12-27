using RabbitRelink.Consumer;
using RabbitRelink.Messaging;

namespace RabbitRelink.Middlewares;

public delegate Task<Acknowledge> ConsumerHandler<T>(ConsumedMessage<T> msg) where T : class?;
