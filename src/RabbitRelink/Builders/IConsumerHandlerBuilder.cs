using RabbitRelink.Consumer;
using RabbitRelink.Messaging;

namespace RabbitRelink;

public interface IConsumerHandlerBuilder
{
    IRelinkConsumer Handler(Func<ConsumedMessage<byte[]>, Task<Acknowledge>> handler);
}
