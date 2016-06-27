using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Utilities
{
    internal static class TaskExtensions
    {
        public async static Task<TResult> CastTask<TResult>(this Task<object> task)
        {
            var result = await task;
            return (TResult)result;
        }

        public static Task<T> Retry<T>(this Func<Task<T>> taskCreate, Func<Task<T>, Task<bool>> policy, int retryLimit)
        {
            return taskCreate().ContinueWith(async t =>
            {
                var task = t;
                if (retryLimit > 0 && await policy(task))
                {
                    return await Retry(taskCreate, policy, retryLimit--);
                }
                else
                {
                    return await task;
                }
            }).Unwrap();
        }
        public static Task Retry(this Func<Task> taskCreate, Func<Task, bool> policy, int retryLimit)
        {
            return taskCreate().ContinueWith(t =>
            {
                var task = t;
                if (retryLimit > 0 && policy(task))
                {
                    return Retry(taskCreate, policy, retryLimit--);
                }
                else {
                    return task;
                }
            }).Unwrap();
        }
    }
}
