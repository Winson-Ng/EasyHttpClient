using EasyHttpClient.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyHttpClient
{
    public class EasyHttpClientFactory
    {
        public IProxyFactory ProxyFactory { get; set; } = new ReflectionProxyFactory();

        public IHttpClientProvider HttpClientProvider { get; set; } = new DefaultHttpClientProvider();

        public EasyClientConfig Config { get; set; } = new EasyClientConfig();


        public T Create<T>(string host)
        {
            return Create<T>(new Uri(host));
        }

        public T Create<T>(Uri host)
        {
            Config.Host = host;
            return Create<T>();
        }

        public T Create<T>()
        {
            Config.HttpClient=HttpClientProvider.GetClient(Config.HttpClientSettings, Config.HttpClientSettings.DelegatingHandlers.Select(i => i()).ToArray());
            return ProxyFactory.Create<T>(new ProxyMethodExecutor(Config));
        }
    }
}
