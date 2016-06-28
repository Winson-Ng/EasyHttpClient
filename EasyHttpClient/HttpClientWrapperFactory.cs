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
        public IOAuth2ClientHandler OAuth2ClientHandler { get; set; }
        public int MaxRetry { get; set; }
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

        #region Type parameter

        public object CreateFor(Type objectType)
        {
            return CreateFor(objectType, DefaultHost);
        }

        public object CreateFor(Type objectType, string host)
        {
            return CreateFor(objectType, new Uri(host));
        }
        #endregion

        #region GenericType
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
            return (T)CreateFor(typeof(T), host);
        }
        #endregion
        public object CreateFor(Type type, Uri host)
        {
            var handlers = new List<DelegatingHandler>();

            if (this.HttpClientSettings.MaxRetry > 0)
            {
                handlers.Add(new RetryHttpHandler(this.HttpClientSettings.MaxRetry));
            }

            var client = HttpClientProvider.GetClient(this.HttpClientSettings, handlers.ToArray());
            var proxyClass = new HttpClientWrapper(type, client, host, this.HttpClientSettings);

            return proxyClass.GetTransparentProxy();
        }
    }
}
