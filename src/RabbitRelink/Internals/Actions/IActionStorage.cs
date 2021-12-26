#region Usings

#endregion

namespace RabbitRelink.Internals.Actions
{
    internal interface IActionStorage<TActor> : IDisposable
    {
        Task<T> PutAsync<T>(Func<TActor, T> action, CancellationToken cancellation);
        Task PutAsync(Action<TActor> action, CancellationToken cancellation);

        ActionItem<TActor> Wait(CancellationToken cancellation);
        Task<ActionItem<TActor>> WaitAsync(CancellationToken cancellation);
    }
}
