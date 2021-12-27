using Microsoft.Extensions.Logging;

namespace RabbitRelink.Logging.Microsoft;

public class RelinkLoggerFactory : IRelinkLoggerFactory
{
    private readonly ILoggerFactory _factory;

    public RelinkLoggerFactory(ILoggerFactory factory)
    {
        _factory = factory;
    }

    public IRelinkLogger CreateLogger(string name)
        => new Logger(_factory.CreateLogger($"RabbitRelink::{name}"));

    private class Logger : IRelinkLogger
    {
        private readonly ILogger _logger;

        public Logger(ILogger logger)
        {
            _logger = logger;
        }

        public void Write(RelinkLoggerLevel level, Exception? ex, string message)
        {
            switch (level)
            {
                case RelinkLoggerLevel.Error:
                    if(ex == null)
                        _logger.LogError("{Message}", message);
                    else
                        _logger.LogError(ex,"{Message}", message);
                    break;
                case RelinkLoggerLevel.Warning:
                    if(ex == null)
                        _logger.LogWarning("{Message}", message);
                    else
                        _logger.LogWarning(ex, "{Message}", message);
                    break;
                case RelinkLoggerLevel.Info:
                    if(ex == null)
                        _logger.LogInformation("{Message}", message);
                    else
                        _logger.LogInformation(ex, "{Message}", message);
                    break;
                case RelinkLoggerLevel.Debug:
                    if(ex == null)
                        _logger.LogDebug("{Message}", message);
                    else
                        _logger.LogDebug(ex, "{Message}", message);
                    break;
                default:
                    if(ex == null)
                        _logger.LogError("{Level} is unknown: {Message}", level, message);
                    else
                        _logger.LogError(ex,"{Level} is unknown: {Message}", level, message);
                    break;
            }
        }
    }
}
