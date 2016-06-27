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

namespace SimpleApiServerTest.Configuration
{
    public static partial class Config
    {
        public static IAppBuilder UseAutofac(this IAppBuilder app, HttpConfiguration httpConfig)
        {
            app.UseAutofacMiddleware(httpConfig.SetupAutofacContainer());
            app.UseAutofacWebApi(httpConfig);
            return app;
        }

        public static IContainer SetupAutofacContainer(this HttpConfiguration httpConfig)
        {
            var builder = new ContainerBuilder();


            var loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterComponents(loadedAssemblies);
            builder.RegisterWebApiModelBinders(loadedAssemblies);
            builder.RegisterApiControllers(loadedAssemblies);
            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterWebApiFilterProvider(httpConfig);
            var container = builder.Build();

            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }
    }
}