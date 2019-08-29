using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EasyHttpClient.Proxy
{
    public class ReflectionProxyFactory : DispatchProxy , IProxyFactory
    {
        private ProxyMethodExecutor _methodExecutor;

        public T Create<T>(ProxyMethodExecutor methodExecutor)
        {
            object instance = Create<T, ReflectionProxyFactory>();
            ((ReflectionProxyFactory)instance).SetParameters(methodExecutor);
            return (T)instance;
        }

        private void SetParameters(ProxyMethodExecutor methodExecutor)
        {
            _methodExecutor = methodExecutor;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _methodExecutor.Excute(targetMethod, args);
        }
    }
}
