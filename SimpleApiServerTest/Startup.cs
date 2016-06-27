using Microsoft.Owin;
using SimpleApiServerTest;
using SimpleApiServerTest.Configuration;

[assembly: OwinStartup(typeof(SimpleApiServerTest.Startup))]
namespace SimpleApiServerTest
{
    using System.Web.Http;
    using Microsoft.Owin;
    using Microsoft.Owin.Extensions;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;
    using Microsoft.Owin.Security.OAuth;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfiguration = new HttpConfiguration();

            app.UseAutofac(httpConfiguration);
            app.UseOAuth(httpConfiguration);
            app.UseWebServer(httpConfiguration);
            // Make ./public the default root of the static files in our Web Application.
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(string.Empty),
                FileSystem = new PhysicalFileSystem("./public"),
                EnableDirectoryBrowsing = true,
            });

            app.UseStageMarker(PipelineStage.MapHandler);

        }
    }
}
