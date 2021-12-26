namespace RabbitRelink.Internals.Actions
{
    interface ICompositeActionStorage<TActor> : IActionStorage<TActor>
    {
        void PutRetry(IEnumerable<ActionItem<TActor>> items, CancellationToken cancellation);
        Task PutRetryAsync(IEnumerable<ActionItem<TActor>> items, CancellationToken cancellation);
        void Yield(CancellationToken cancellation);
        Task YieldAsync(CancellationToken cancellation);
    }
}
