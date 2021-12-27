using RabbitRelink.Consumer;

namespace RabbitRelink;

public interface IConsumerConfigBuilder
{
    IRelinkPullConsumer Pull(Func<PullConsumerConfig, PullConsumerConfig>? configure = null);
    IConsumerHandlerBuilder Push(Func<PushConsumerConfig, PushConsumerConfig>? configure = null);
}
