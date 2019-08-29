using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TimeoutAttribute : ActionFilterAttribute
    {
        public TimeoutAttribute()
        {

        }
        public TimeoutAttribute(int _timeoutMilliseconds)
        {
            this.TimeoutMilliseconds = _timeoutMilliseconds;
        }
        public int TimeoutMilliseconds { get; set; }

        public override Task<EasyHttpClient.IHttpResult> ActionInvoke(EasyHttpClient.ActionContext context, Func<Task<EasyHttpClient.IHttpResult>> continuation)
        {
            if (this.TimeoutMilliseconds > 0)
            {
                context.CancellationToken.CancelAfter(this.TimeoutMilliseconds);
                return base.ActionInvoke(context, continuation);
            }
            else
            {
                return base.ActionInvoke(context, continuation);
            }
        }
    }
}
