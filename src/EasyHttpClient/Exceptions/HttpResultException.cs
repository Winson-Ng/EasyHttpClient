using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Exceptions
{
    public class HttpResultException : Exception
    {
        public IHttpResult HttpResult { get; private set; }
        public HttpResultException(IHttpResult httpResult, Exception ex)
            : base("HttpResult parse fail, httpCode:" 
            + httpResult.StatusCode 
            + (httpResult.RequestMessage != null ? (", url:" + httpResult.RequestMessage.RequestUri) : "")
            , ex)
        {
            this.HttpResult = httpResult;
        }
    }
}
