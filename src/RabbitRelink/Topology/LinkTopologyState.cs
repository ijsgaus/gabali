namespace RabbitRelink.Topology
{
    /// <summary>
    ///     State of <see cref="IRelinkTopology" />
    /// </summary>
    public enum LinkTopologyState
    {
        /// <summary>
        ///     Initializing
        /// </summary>
        Init,

        /// <summary>
        ///     Configuring channel and topology
        /// </summary>
        Configuring,

        /// <summary>
        ///     Reconfiguring channel and topology
        /// </summary>
        Reconfiguring,

        /// <summary>
        ///     Topology successfully configured
        /// </summary>
        Ready,

        /// <summary>
        ///     Stopping
        /// </summary>
        Stopping,

        /// <summary>
        ///     Disposed
        /// </summary>
        Disposed
    }
}
