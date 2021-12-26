﻿#region Usings

#endregion

namespace RabbitRelink.Internals.Async
{
    internal static class AsyncHelper
    {
        public static Task RunAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        public static Task<TResult> RunAsync<TResult>(Func<TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return Task.Factory.StartNew(func, TaskCreationOptions.LongRunning);
        }
    }
}
