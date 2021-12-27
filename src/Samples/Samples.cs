using System.Text;
using Microsoft.Extensions.Logging;
using RabbitRelink;
using RabbitRelink.Consumer;
using RabbitRelink.Logging.Microsoft;
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
            .Handler(msg =>
            {
                var str = Encoding.UTF8.GetString(msg.Body);
                logger.LogInformation("RECEIVED: {Message}", str);
                logger.LogInformation("PROPERTIES: {Properties}", msg.Properties);
                logger.LogInformation("RECEIVED_PROPERTIES: {ReceivedProperties}", msg.ReceiveProperties);
                logger.LogInformation("EQUAL: {Ok}", str == "OK");
                _ = Task.Delay(200).ContinueWith(_ => source.TrySetResult());
                return Task.FromResult(Acknowledge.Ack);
            });
        await consumer.WaitReadyAsync();
        using var producer = relink.Producer()
            .Exchange(cfg => cfg.ExchangeDeclare("test-exchange", ExchangeType.Direct, true, true))
            .Configure(p => p with {ConfirmsMode = true})
            .Build();
        await producer.PublishAsync(Encoding.UTF8.GetBytes("OK"), p => p, p => p with {RoutingKey = "test.key"});
        await source.Task;
    }
}
