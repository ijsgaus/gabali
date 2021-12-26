#region Usings

#endregion

using RabbitRelink.Messaging;

namespace RabbitRelink.Consumer
{
    /// <summary>
    ///     Represents RabbitMQ message consumer which manage internal message queue
    ///     and implements semantic
    /// </summary>
    public interface IRelinkPullConsumer : IRelinkConsumer, IAsyncEnumerable<PulledMessage<byte[]>>
    {

    }
}
