using RabbitRelink.Producer;

namespace RabbitRelink;

public interface IProducerConfigBuilder
{
    IRelinkProducer Configure(Func<ProducerConfig, ProducerConfig> configure);
    IRelinkProducer Build();
}
