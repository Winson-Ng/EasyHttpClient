using EasyHttpClient.DelegatingHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public interface IHttpClientProvider
    {
        HttpClient GetClient(HttpClientSettings clientSetting, params DelegatingHandler[] handlers);
    }

    public class DefaultHttpClientProvider : IHttpClientProvider
    {
        public virtual HttpClient GetClient(HttpClientSettings clientSetting, params DelegatingHandler[] handlers)
        {
            return HttpClientFactory.Create(handlers);
        }
    }
}
