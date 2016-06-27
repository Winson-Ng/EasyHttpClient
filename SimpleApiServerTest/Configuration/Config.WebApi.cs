using Common.Logging;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using SimpleApiServerTest.Code.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Batch;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;

namespace SimpleApiServerTest.Configuration
{
    public static partial class Config
    {
        public static IAppBuilder UseWebServer(this IAppBuilder builder, HttpConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            var httpServer = new HttpServer(config);

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //Web API DelegatingHandler
            //config.MessageHandlers.Add(new MonitorLog.MonitorLogHandler());
            // Web API routes
            var constraintResolver = new DefaultInlineConstraintResolver();

            config.Routes.MapHttpBatchRoute(
                routeName: "batch",
                routeTemplate: "api/batch",
                batchHandler: new DefaultHttpBatchHandler(httpServer)
            );

            config.MapHttpAttributeRoutes(constraintResolver);
            //config.MessageHandlers.Add(new CultureAppLangHandler());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            // Global unhandled exception logger
            config.Services.Add(typeof(IExceptionLogger), new CommonLoggingExceptionLogger());

            // Remove XML formatter by default.
            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            // Set default Json formatting.
            var jsonSettings = formatters.JsonFormatter.SerializerSettings;
            jsonSettings.Formatting = Formatting.Indented;
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            builder.UseWebApi(httpServer);

            return builder;
        }
    }
}