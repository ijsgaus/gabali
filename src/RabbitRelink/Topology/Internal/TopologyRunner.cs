#region Usings

using RabbitMQ.Client;
using RabbitRelink.Internals.Actions;
using RabbitRelink.Internals.Async;
using RabbitRelink.Internals.Lens;
using RabbitRelink.Logging;

#endregion

namespace RabbitRelink.Topology.Internal
{
    internal class TopologyRunner<T>
    {
        #region Fields

        private readonly Func<ITopologyCommander, Task<T>> _configureFunc;
        private readonly IRelinkLogger _logger;

        #endregion

        #region Ctor

        public TopologyRunner(IRelinkLogger logger, Func<ITopologyCommander, Task<T>> configureFunc)
        {
            _configureFunc = configureFunc;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        public async Task<T> RunAsync(IModel model, CancellationToken cancellation)
        {
            var queue = new ActionStorage<IModel>(new LensChannel<ActionItem<IModel>>());
            var configTask = RunConfiguration(queue, cancellation);

            await StartQueueWorker(model, queue, cancellation)
                .ConfigureAwait(false);

            return await configTask
                .ConfigureAwait(false);
        }

        private Task StartQueueWorker(IModel model, IActionStorage<IModel> storage,
            CancellationToken cancellation)
        {
            return AsyncHelper.RunAsync(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    ActionItem<IModel> item;
                    try
                    {
                        item = storage.Wait(cancellation);
                    }
                    catch
                    {
                        break;
                    }

                    try
                    {
                        var result = item.Value(model);
                        item.TrySetResult(result);
                    }
                    catch (Exception ex)
                    {
                        item.TrySetException(ex);
                    }
                }
            });
        }

        private async Task<T> RunConfiguration(IActionStorage<IModel> storage, CancellationToken cancellation)
        {
            try
            {
                return await Task.Run(() =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        var invoker = new ActionInvoker<IModel>(storage, cancellation);
                        var config = new TopologyCommander(_logger, invoker);
                        return _configureFunc(config);
                    }, cancellation)
                    .ConfigureAwait(false);
            }
            finally
            {
                storage.Dispose();
            }
        }
    }
}
