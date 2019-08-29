using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ActionFilterAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// The smaller is higher priority
        /// </summary>
        public int Order { get; set; }

        public virtual Task<IHttpResult> ActionInvoke(ActionContext context, Func<Task<IHttpResult>> continuation)
        {
            return continuation();
        }
    }
}
