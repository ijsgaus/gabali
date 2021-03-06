#region Usings

#endregion

namespace RabbitRelink.Topology
{
    /// <summary>
    /// Topology configurator
    /// </summary>
    public interface ITopologyCommander
    {
        /// <summary>
        /// Declare new exchange or return already existent if all settings match
        /// </summary>
        /// <param name="name">name of exchange</param>
        /// <param name="type">type of exchange</param>
        /// <param name="durable">is exchange durable</param>
        /// <param name="autoDelete">exchange must be deleted after all queues disconnect</param>
        /// <param name="alternateExchange">alternative exchange for messages which cannot be routed by this exchange</param>
        /// <param name="delayed">is exchange can handler delayed messages (need plugin in RabbitMQ)</param>
        Task<IExchange> ExchangeDeclare(
            string name,
            ExchangeType type,
            bool durable = true,
            bool autoDelete = false,
            string? alternateExchange = null,
            bool delayed = false
        );

        /// <summary>
        /// Returns already declared exchange by name
        /// </summary>
        /// <param name="name">name of exchange</param>
        Task<IExchange> ExchangeDeclarePassive(string name);

        /// <summary>
        /// Returns default RabbitMQ exchange
        /// </summary>
        Task<IExchange> ExchangeDeclareDefault();

        /// <summary>
        /// Deletes exchange
        /// </summary>
        /// <param name="exchange">exchange for delete</param>
        /// <param name="ifUnused">delete only if exchange is unused</param>
        /// <returns></returns>
        Task ExchangeDelete(IExchange exchange, bool ifUnused = false);

        /// <summary>
        /// Declare exclusive queue with name generated by server
        /// </summary>
        Task<IQueue> QueueDeclareExclusiveByServer();

        /// <summary>
        /// Declare exclusive queue with name generated by library
        /// </summary>
        /// <param name="autoDelete">is queue must be delete when all consumers are gone</param>
        /// <param name="messageTtl">ttl for messages</param>
        /// <param name="expires">timeout to delete queue when all consumer are gone</param>
        /// <param name="maxPriority">maximum priority of message</param>
        /// <param name="maxLength">maximum length of queue</param>
        /// <param name="maxLengthBytes">maximum length of queue in bytes</param>
        /// <param name="deadLetterExchange">exchange where message will be passed when NACKed by consumer</param>
        /// <param name="deadLetterRoutingKey">routing key for message when NACKed by consumer and routed to dead letter exchange</param>
        Task<IQueue> QueueDeclareExclusive(
            bool autoDelete = true,
            TimeSpan? messageTtl = null,
            TimeSpan? expires = null,
            byte? maxPriority = null,
            int? maxLength = null,
            int? maxLengthBytes = null,
            string? deadLetterExchange = null,
            string? deadLetterRoutingKey = null
        );

        /// <summary>
        /// Declare exclusive queue with name generated by library with prefix specified by caller
        /// </summary>
        /// <param name="prefix">name prefix</param>
        /// <param name="autoDelete">is queue must be delete when all consumers are gone</param>
        /// <param name="messageTtl">ttl for messages</param>
        /// <param name="expires">timeout to delete queue when all consumer are gone</param>
        /// <param name="maxPriority">maximum priority of message</param>
        /// <param name="maxLength">maximum length of queue</param>
        /// <param name="maxLengthBytes">maximum length of queue in bytes</param>
        /// <param name="deadLetterExchange">exchange where message will be passed when NACKed by consumer</param>
        /// <param name="deadLetterRoutingKey">routing key for message when NACKed by consumer and routed to dead letter exchange</param>
        Task<IQueue> QueueDeclareExclusive(
            string prefix,
            bool autoDelete = true,
            TimeSpan? messageTtl = null,
            TimeSpan? expires = null,
            byte? maxPriority = null,
            int? maxLength = null,
            int? maxLengthBytes = null,
            string? deadLetterExchange = null,
            string? deadLetterRoutingKey = null
        );

        /// <summary>
        /// Returns already declared queue by name
        /// </summary>
        Task<IQueue> QueueDeclarePassive(string name);


        /// <summary>
        /// Declare new queue or return already existent if all settings match
        /// </summary>
        /// <param name="name">name of queue</param>
        /// <param name="durable">is queue durable</param>
        /// <param name="exclusive">is queue exclusive (may be used by only one consumer)</param>
        /// <param name="autoDelete">is queue must be delete when all consumers are gone</param>
        /// <param name="messageTtl">ttl for messages</param>
        /// <param name="expires">timeout to delete queue when all consumer are gone</param>
        /// <param name="maxPriority">maximum priority of message</param>
        /// <param name="maxLength">maximum length of queue</param>
        /// <param name="maxLengthBytes">maximum length of queue in bytes</param>
        /// <param name="deadLetterExchange">exchange where message will be passed when NACKed by consumer</param>
        /// <param name="deadLetterRoutingKey">routing key for message when NACKed by consumer and routed to dead letter exchange</param>
        Task<IQueue> QueueDeclare(
            string name,
            bool durable = true,
            bool exclusive = false,
            bool autoDelete = false,
            TimeSpan? messageTtl = null,
            TimeSpan? expires = null,
            byte? maxPriority = null,
            int? maxLength = null,
            int? maxLengthBytes = null,
            string? deadLetterExchange = null,
            string? deadLetterRoutingKey = null
        );

        /// <summary>
        /// Deletes queue
        /// </summary>
        /// <param name="queue">queue for delete</param>
        /// <param name="ifUnused">delete queue only if it have no consumers</param>
        /// <param name="ifEmpty">delete queue only if no messages in it</param>
        /// <returns></returns>
        Task QueueDelete(IQueue queue, bool ifUnused = false, bool ifEmpty = false);

        /// <summary>
        /// Purge queue (remove all messages)
        /// </summary>
        /// <param name="queue">queue for purge</param>
        Task QueuePurge(IQueue queue);

        /// <summary>
        /// Binds one exchange to another
        /// </summary>
        /// <param name="destination">destination exchange (consumer)</param>
        /// <param name="source">source exchange (producer)</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="arguments">additional arguments</param>
        /// <returns></returns>
        Task Bind(IExchange destination, IExchange source, string? routingKey = null,
            IDictionary<string, object>? arguments = null);

        /// <summary>
        /// Unbind one exchange from another
        /// </summary>
        /// <param name="destination">destination exchange (consumer)</param>
        /// <param name="source">source exchange (producer)</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="arguments">additional arguments</param>
        /// <returns></returns>
        Task Unbind(IExchange destination, IExchange source, string? routingKey = null,
            IDictionary<string, object>? arguments = null);

        /// <summary>
        /// Binds queue to exchange
        /// </summary>
        /// <param name="queue">queue (consumer)</param>
        /// <param name="exchange">exchange (producer)</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="arguments">additional arguments</param>
        /// <returns></returns>
        Task Bind(IQueue queue, IExchange exchange, string? routingKey = null,
            IDictionary<string, object>? arguments = null);


        /// <summary>
        /// Unbind queue from exchange
        /// </summary>
        /// <param name="queue">queue (consumer)</param>
        /// <param name="exchange">exchange (producer)</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="arguments">additional arguments</param>
        /// <returns></returns>
        Task Unbind(IQueue queue, IExchange exchange, string? routingKey = null,
            IDictionary<string, object>? arguments = null);
    }
}
