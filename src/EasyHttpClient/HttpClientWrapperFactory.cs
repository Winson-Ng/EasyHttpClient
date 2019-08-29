using EasyHttpClient.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class HttpClientWrapperFactory
    {
        public Uri Host { get; set; }

        public IHttpClientProvider HttpClientProvider { get; set; }

        public HttpClientSettings HttpClientSettings { get; private set; }

        public bool AuthorizeRequired { get; set; }

        public HttpClientWrapperFactory()
        {
            HttpClientSettings = new HttpClientSettings();
            HttpClientProvider = new DefaultHttpClientProvider();
        }

        public T CreateFor<T>()
        {
            var client = HttpClientProvider.GetClient(this.HttpClientSettings, this.HttpClientSettings.DelegatingHandlers.Select(i => i()).ToArray());
            return HttpClientWrapper.Create<T>(this);
        }

    }
}
