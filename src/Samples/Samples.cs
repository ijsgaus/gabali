using System.Text;
using Microsoft.Extensions.Logging;
using RabbitRelink;
using RabbitRelink.Consumer;
using RabbitRelink.Logging.Microsoft;
using RabbitRelink.Serialization.Abstractions;
using RabbitRelink.Topology;

namespace Samples;

public class Samples
{
    public static async Task SimplePublishSubscribe(ILoggerFactory factory)
    {
        var logger = factory.CreateLogger<Samples>();
        var source = new TaskCompletionSource();
        using var relink = Relink.Create("amqp://admin:admin@localhost:5672")
            .Configure(cfg =>
                cfg with
                {
                    AppId = "TestSamples",
                    ConnectionName = $"TestSamples:{Environment.MachineName}"
                })
            .UseMicrosoftLogging(factory)
            .Build();
        using var consumer = relink.Consumer()
            .Queue(async cfg =>
            {
                var exchange = await cfg.ExchangeDeclare("test-exchange", ExchangeType.Direct, true, true);
                var queue = await cfg.QueueDeclare("test-queue", expires: TimeSpan.FromMinutes(1));
                await cfg.Bind(queue, exchange, "test.key");
                return queue;
            })
            .Push(p => p with
            {
                Parallelism = 1,
                PrefetchCount = 5,
            })
            .UsePlainText()
            .Handler(msg =>
            {
                logger.LogInformation("RECEIVED: {Message}", msg.Body);
                logger.LogInformation("PROPERTIES: {Properties}", msg.Properties);
                logger.LogInformation("RECEIVED_PROPERTIES: {ReceivedProperties}", msg.ReceiveProperties);
                logger.LogInformation("EQUAL: {Ok}", msg.Body == "OK");
                _ = Task.Delay(200).ContinueWith(_ => source.TrySetResult());
                return Task.FromResult(Acknowledge.Ack);
            });
        await consumer.WaitReadyAsync();
        using var producer = relink.Producer()
            .Exchange(cfg => cfg.ExchangeDeclare("test-exchange", ExchangeType.Direct, true, true))
            .Configure(p => p with {ConfirmsMode = true})
            .UsePlainText()
            .Build();
        await producer.PublishAsync("OK", p => p, p => p with {RoutingKey = "test.key"});
        await source.Task;
    }
}
