namespace RabbitRelink.Exceptions
{
    /// <summary>
    /// Fired when published message was Returned
    /// </summary>
    public class MessageReturnedException : MessagePublishException
    {
        /// <summary>
        /// Constructs instance with reason
        /// </summary>
        /// <param name="reason"></param>
        public MessageReturnedException(string reason) : base($"Message Returned: {reason}")
        {
        }
    }
}
