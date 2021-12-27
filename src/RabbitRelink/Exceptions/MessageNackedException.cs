namespace RabbitRelink.Exceptions
{
    /// <summary>
    ///     Fired when published message was NACKed
    /// </summary>
    public class MessageNackedException : MessagePublishException
    {
        /// <summary>
        ///     Constructs instance
        /// </summary>
        public MessageNackedException() : base("Message NACKed")
        {
        }
    }
}
