namespace RabbitRelink.Connection
{
    /// <summary>
    /// Operational state of <see cref="IRelinkChannel"/>
    /// </summary>
    public enum RelinkChannelState
    {
        /// <summary>
        /// Waiting for initialization
        /// </summary>
        Init,

        /// <summary>
        /// Opening
        /// </summary>
        Opening,

        /// <summary>
        /// Reopening
        /// </summary>
        Reopening,

        /// <summary>
        /// Active processing
        /// </summary>
        Active,

        /// <summary>
        /// Stoping
        /// </summary>
        Stopping,

        /// <summary>
        /// Disposed
        /// </summary>
        Disposed
    }
}
