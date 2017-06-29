using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    public interface IHttpResultDecoder
    {
        bool CanDecode(HttpContentHeaders headers);
        Task<object> DecodeAsync(HttpContent httpContent, ActionContext actionContext);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class HttpResultDecoderAttribute : Attribute, IHttpResultDecoder
    {
        public abstract bool CanDecode(HttpContentHeaders headers);

        public abstract Task<object> DecodeAsync(HttpContent httpContent, ActionContext actionContext);
    }
}
