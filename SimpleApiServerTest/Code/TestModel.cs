//using Autofac.Extras.DynamicProxy2;
//using SimpleApiServerTest.Code.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleApiServerTest.Code
{
    //[Intercept(typeof(CacheInterceptor))]
    public class TestModel
    {
        public virtual string Dosomething()
        {
            return "Done";
        }

        public virtual Task<string> DosomethingAsync()
        {
            Func<string> result = () =>
            {
                return "Async Done";
            };
            return Task.Run<string>(result);
        }
    }
}