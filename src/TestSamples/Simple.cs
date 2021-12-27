using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RabbitRelink;
using RabbitRelink.Consumer;
using RabbitRelink.Logging.Microsoft;
using RabbitRelink.Topology;
using Xunit;
using Xunit.Abstractions;

namespace TestSamples;

public class Simple
{
    private readonly ITestOutputHelper _outputHelper;

    public Simple(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Fact]
    public async Task ConnectPublishSubscribe()
    {
        var services = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .BuildServiceProvider();
        var factory = services.GetRequiredService<ILoggerFactory>();
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
                var str = Encoding.UTF8.GetString(msg.Body ?? Array.Empty<byte>());
                _outputHelper.WriteLine("RECEIVED: {0}", str);
                _outputHelper.WriteLine("PROPERTIES: {0}", msg.Properties);
                _outputHelper.WriteLine("RECIVED_PROPERTIES: {0}", msg.ReceiveProperties);
                Assert.Equal("OK", str);
                _ = Task.Delay(200).ContinueWith(p => source.TrySetResult());
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
