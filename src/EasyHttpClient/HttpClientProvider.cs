using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            HttpClient httpClient = null;
            if (clientSetting.AutomaticDecompression)
            {
                var internalHandler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                httpClient= HttpClientFactory.Create(internalHandler, handlers);
            }
            else
            {
                httpClient= HttpClientFactory.Create(handlers);
            }

            httpClient.Timeout = clientSetting.Timeout;

            return httpClient;
        }
    }
}
