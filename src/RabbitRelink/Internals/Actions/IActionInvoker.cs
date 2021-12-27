#region Usings

#endregion

namespace RabbitRelink.Internals.Actions
{
    internal interface IActionInvoker<out TActor>
    {
        Task InvokeAsync(Action<TActor> action, CancellationToken cancellation);
        Task<T?> InvokeAsync<T>(Func<TActor, T?> action, CancellationToken cancellation);
    }
}
