using System;
using System.Collections.Generic;
using System.Text;

namespace EasyHttpClient.Proxy
{
    public interface IProxyFactory
    {
        T Create<T>(ProxyMethodExecutor methodExecutor);
    }
}
