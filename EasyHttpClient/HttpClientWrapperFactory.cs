using EasyHttpClient.DelegatingHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class HttpClientSettings
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        public IOAuth2ClientHandler OAuth2ClientProvider { get; set; }
        public int MaxRetry { get; set; }
        private JsonSerializerSettings _jsonSerializerSettings;
        public JsonSerializerSettings JsonSerializerSettings
        {
            get;
            set;
        }

        internal HttpClientSettings()
        {
            this.JsonSerializerSettings = DefaultJsonSerializerSettings;
        }
    }
    public class HttpClientWrapperFactory
    {
        public Uri DefaultHost { get; set; }

        public IHttpClientProvider HttpClientProvider { get; set; }

        public HttpClientSettings HttpClientSettings { get; private set; }

        public HttpClientWrapperFactory()
        {
            HttpClientSettings = new HttpClientSettings();
            HttpClientProvider = new DefaultHttpClientProvider();
        }

        public T CreateFor<T>()
        {
            return CreateFor<T>(DefaultHost);
        }

        public T CreateFor<T>(string host)
        {
            return CreateFor<T>(new Uri(host));
        }

        public T CreateFor<T>(Uri host)
        {
            var handlers = new List<DelegatingHandler>();

            if (this.HttpClientSettings.MaxRetry > 0)
            {
                handlers.Add(new RetryHttpHandler(this.HttpClientSettings.MaxRetry));
            }

            if (this.HttpClientSettings.OAuth2ClientProvider != null)
            {
                handlers.Add(new OAuth2HttpHandler(this.HttpClientSettings.OAuth2ClientProvider));
            }

            var client = HttpClientProvider.GetClient(handlers.ToArray());
            var proxyClass = new HttpClientWrapper<T>(client, host, this.HttpClientSettings);

            return (T)proxyClass.GetTransparentProxy();
        }
    }
}
