#region Usings

#endregion

using RabbitRelink.Connection;

namespace RabbitRelink.Producer
{
    internal interface IRelinkProducerInternal : IRelinkProducer<byte[]?>
    {
        event EventHandler? Disposed;

        IRelinkChannel Channel { get; }
    }
}
