using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPatchAttribute:Attribute, IHttpMethodAttribute
    {
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");
        public HttpMethod HttpMethod
        {
            get {
                return Patch;
            }
        }
    }
}
