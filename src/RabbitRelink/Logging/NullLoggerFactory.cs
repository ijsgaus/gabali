namespace RabbitRelink.Logging
{
    /// <summary>
    ///     Implementation of <see cref="IRelinkLoggerFactory" /> which provide noop logger
    /// </summary>
    public sealed class NullLoggerFactory : IRelinkLoggerFactory
    {
        /// <inheritdoc />
        public IRelinkLogger CreateLogger(string name)
        {
            return new Logger();
        }

        private class Logger : IRelinkLogger
        {
            public void Write(RelinkLoggerLevel level, Exception? ex, string message)
            {
            }
        }
    }
}
