using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyHttpClient.Utilities
{
    internal static class TaskExtensions
    {
        public async static Task<TResult> CastHttpResultTask<TResult>(this Task<IHttpResult> task)
        {
            var result = await task;
            return (TResult)result;
        }

        public async static Task<TResult> CastObjectTask<TResult>(this Task<IHttpResult> task)
        {
            var result = await task;
            return (TResult)result.Content;
        }

        public static Task<T> Retry<T>(this Func<Task<T>> taskCreate, Func<T, Task<bool>> policy, int retryLimit)
        {
            return taskCreate().Then(async r=>
            {
                if (retryLimit > 0 && await policy(r))
                {
                    return await Retry(taskCreate, policy, retryLimit-1);
                }
                else
                {
                    return r;
                }
            });
        }
        public static Task Retry(this Func<Task> taskCreate, int retryLimit)
        {
            return taskCreate().Then(() =>
            {
                if (retryLimit > 0 )
                {
                    return Retry(taskCreate, retryLimit-1);
                }
                else {
                    return Task.FromResult(0);
                }
            });
        }
    }
}
