// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection()
    .AddLogging(builder =>
        builder.SetMinimumLevel(LogLevel.Trace)
            .AddConsole())
    .BuildServiceProvider();
var factory = services.GetRequiredService<ILoggerFactory>();
await Samples.Samples.SimplePublishSubscribe(factory);

