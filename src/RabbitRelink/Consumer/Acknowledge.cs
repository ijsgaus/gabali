namespace RabbitRelink.Consumer
{
    /// <summary>
    /// Consumer ACK strategies
    /// </summary>
    public enum Acknowledge
    {
        /// <summary>
        /// ACK message
        /// </summary>
        Ack,

        /// <summary>
        /// NACK message
        /// </summary>
        Nack,

        /// <summary>
        /// Requeue message
        /// </summary>
        Requeue
    }
}
