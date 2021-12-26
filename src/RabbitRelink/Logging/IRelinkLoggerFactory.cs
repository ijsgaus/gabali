namespace RabbitRelink.Logging
{
    /// <summary>
    ///     Factory for <see cref="IRelinkLogger" />
    /// </summary>
    public interface IRelinkLoggerFactory
    {
        /// <summary>
        ///     Gets new instance of <see cref="IRelinkLogger" />
        /// </summary>
        /// <param name="name">Name of <see cref="IRelinkLogger" /></param>
        /// <returns>new <see cref="IRelinkLogger" /> instance</returns>
        IRelinkLogger CreateLogger(string name);
    }
}
