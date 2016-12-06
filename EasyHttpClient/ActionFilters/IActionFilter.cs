using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.ActionFilters
{
    public interface IActionFilter
    {
        /// <summary>
        /// The smaller is higher priority
        /// </summary>
        int Order { get; set; }

        Task<IHttpResult> ActionInvoke(ActionContext context, Func<Task<IHttpResult>> Continuation);

    }
}
