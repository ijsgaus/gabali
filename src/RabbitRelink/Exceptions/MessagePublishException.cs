#region Usings

#endregion

namespace RabbitRelink.Exceptions
{
    /// <summary>
    ///     Base class for message publish exceptions
    /// </summary>
    public abstract class MessagePublishException : RelinkException
    {
        #region Ctor

        /// <summary>
        ///     Constructs instance with message
        /// </summary>
        /// <param name="message">message</param>
        protected MessagePublishException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Constructs instance with message and inner exception
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="innerException">inner exception</param>
        protected MessagePublishException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}
