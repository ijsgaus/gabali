#region Usings

#endregion

namespace RabbitRelink.Logging
{
    internal static class RelinkLoggerExtensions
    {
        public static void Error(this IRelinkLogger logger, string message)
            => logger.Write(RelinkLoggerLevel.Error, null, message);

        public static void Error(this IRelinkLogger logger, Exception exception, string message)
            => logger.Write(RelinkLoggerLevel.Error, exception, message);

        public static void Warning(this IRelinkLogger logger, string message)
            => logger.Write(RelinkLoggerLevel.Warning, null, message);

        public static void Warning(this IRelinkLogger logger, Exception exception, string message)
            => logger.Write(RelinkLoggerLevel.Warning, exception, message);

        public static void Info(this IRelinkLogger logger, string message)
            => logger.Write(RelinkLoggerLevel.Info, null, message);

        public static void Info(this IRelinkLogger logger, Exception exception, string message)
            => logger.Write(RelinkLoggerLevel.Info, exception, message);

        public static void Debug(this IRelinkLogger logger, string message)
            => logger.Write(RelinkLoggerLevel.Debug, null, message);

        public static void Debug(this IRelinkLogger logger, Exception exception, string message)
            => logger.Write(RelinkLoggerLevel.Debug, exception, message);

        public static IRelinkLogger CreateLogger<T>(this IRelinkLoggerFactory factory)
            => factory.CreateLogger(typeof(T).FullName ?? "(unknown type name)");

    }
}
