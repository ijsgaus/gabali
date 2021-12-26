namespace RabbitRelink.Topology
{
    /// <summary>
    /// Type of RabbitMQ exchange
    /// </summary>
    public enum ExchangeType
    {
        /// <summary>
        /// Direct
        /// </summary>
        Direct,

        /// <summary>
        /// Fanout
        /// </summary>
        Fanout,

        /// <summary>
        /// Topic
        /// </summary>
        Topic,

        /// <summary>
        /// Headers
        /// </summary>
        Headers
    }
}
