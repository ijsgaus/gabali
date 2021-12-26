#region Usings

#endregion

namespace RabbitRelink.Logging
{
    /// <summary>
    ///     Logger interface for <see cref="Link" /> and underlying components
    /// </summary>
    public interface IRelinkLogger
    {
        /// <summary>
        ///     Writes message to log
        /// </summary>
        /// <param name="level">Error level</param>
        /// <param name="message">Message to write</param>
        void Write(RelinkLoggerLevel level, Exception? ex, string message);
    }
}
