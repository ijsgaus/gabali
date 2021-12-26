namespace RabbitRelink.Topology
{
    /// <summary>
    /// Represents RabbitMQ exchange
    /// </summary>
    public interface IExchange
    {
        /// <summary>
        /// Name of exchange
        /// </summary>
        string Name { get; }
    }
}
