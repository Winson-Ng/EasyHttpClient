using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using SimpleApiServerTest.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
//using Autofac.Extras.DynamicProxy2;
//using SimpleApiServerTest.Code.Interceptors;

namespace SimpleApiServerTest.Configuration
{
    public static partial class Config
    {
        /// <summary>
        ///  To Register the Components
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loadedAssemblies"></param>
        public static void RegisterComponents(this ContainerBuilder builder, Assembly[] loadedAssemblies)
        {
            builder.RegisterType<TestModel>();

        }

    }
}