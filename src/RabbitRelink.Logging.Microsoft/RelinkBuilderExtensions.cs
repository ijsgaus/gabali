using Microsoft.Extensions.Logging;

namespace RabbitRelink.Logging.Microsoft;

public static class RelinkBuilderExtensions
{
    public static IRelinkBuilder UseMicrosoftLogging(this IRelinkBuilder builder, ILoggerFactory factory)
        => builder.Configure(p => p with {LoggerFactory = new RelinkLoggerFactory(factory)});
}
