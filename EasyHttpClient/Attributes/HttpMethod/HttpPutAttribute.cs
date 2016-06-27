using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPutAttribute : Attribute, IHttpMethodAttribute
    {
        public HttpMethod HttpMethod
        {
            get
            {
                return HttpMethod.Put;
            }
        }
    }
}
