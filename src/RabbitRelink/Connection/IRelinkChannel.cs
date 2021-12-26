#region Usings

using RabbitMQ.Client;

#endregion

namespace RabbitRelink.Connection
{
    /// <summary>
    ///     Represents <see cref="IModel" /> with automatic recovering
    /// </summary>
    internal interface IRelinkChannel : IDisposable
    {
        #region Properties

        /// <summary>
        ///     Identifier
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Operating state
        /// </summary>
        RelinkChannelState State { get; }

        #endregion

        /// <summary>
        ///     Called when channel disposed
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Initializes channel
        /// </summary>
        /// <param name="handler">Handler to run callbacks on</param>
        void Initialize(IRelinkChannelHandler handler);

        /// <summary>
        /// Channel's connection
        /// </summary>
        IRelinkConnection Connection { get; }
    }
}
