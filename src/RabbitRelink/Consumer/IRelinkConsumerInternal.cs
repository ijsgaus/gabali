using RabbitRelink.Connection;

namespace RabbitRelink.Consumer
{
    internal interface IRelinkConsumerInternal : IRelinkConsumer
    {
        event EventHandler? Disposed;

        IRelinkChannel Channel { get; }
    }
}
