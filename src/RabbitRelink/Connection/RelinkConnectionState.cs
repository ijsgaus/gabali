namespace RabbitRelink.Connection
{
    /// <summary>
    /// Operational state of connection
    /// </summary>
    public enum RelinkConnectionState
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
        /// Active
        /// </summary>
        Active,

        /// <summary>
        /// Stopping
        /// </summary>
        Stopping,

        /// <summary>
        /// Disposed
        /// </summary>
        Disposed
    }
}
