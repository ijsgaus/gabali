#region Usings

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

#endregion

namespace RabbitRelink.Connection
{
    /// <summary>
    ///     Callbacks handler for <see cref="IRelinkChannel" />
    /// </summary>
    internal interface IRelinkChannelHandler
    {
        /// <summary>
        ///     Raises when <see cref="IRelinkChannel" /> enters Active state
        /// </summary>
        /// <param name="model">Active <see cref="IModel" /></param>
        /// <param name="cancellation">Cancellation to stop processing</param>
        /// <returns></returns>
        Task OnActive(IModel model, CancellationToken cancellation);

        /// <summary>
        ///     Raises when <see cref="IRelinkChannel" /> enters Connecting state
        /// </summary>
        /// <param name="cancellation">Cancellation to stop processing</param>
        /// <returns></returns>
        Task OnConnecting(CancellationToken cancellation);

        /// <summary>
        ///     Raises when <see cref="IRelinkChannel" />'s active <see cref="IModel" /> receives ACK to message
        /// </summary>
        /// <param name="info">ACK information</param>
        void MessageAck(BasicAckEventArgs info);

        /// <summary>
        ///     Raises when <see cref="IRelinkChannel" />'s active <see cref="IModel" /> receives NACK to message
        /// </summary>
        /// <param name="info">NACK information</param>
        void MessageNack(BasicNackEventArgs info);

        /// <summary>
        ///     Raises when <see cref="IRelinkChannel" />'s active <see cref="IModel" /> receives RETURN to message
        /// </summary>
        /// <param name="info">RETURN information</param>
        void MessageReturn(BasicReturnEventArgs info);
    }
}
