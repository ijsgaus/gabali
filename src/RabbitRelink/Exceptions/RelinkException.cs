#region Usings

#endregion

namespace RabbitRelink.Exceptions
{
    /// <summary>
    /// Base class for all exceptions in library
    /// </summary>
    public abstract class RelinkException : Exception
    {
        /// <summary>
        /// Construct instance with message
        /// </summary>
        /// <param name="message">message</param>
        protected RelinkException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs instance with message and inner exception
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="innerException">inner exception</param>
        protected RelinkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
