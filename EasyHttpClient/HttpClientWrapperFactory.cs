using EasyHttpClient.OAuth2;
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
            var client = HttpClientProvider.GetClient(this.HttpClientSettings, this.HttpClientSettings.DelegatingHandlers.Select(i=>i()).ToArray());
            var proxyClass = new HttpClientWrapper(type, client, host, this.HttpClientSettings);
            return proxyClass.GetTransparentProxy();
        }
    }
}
