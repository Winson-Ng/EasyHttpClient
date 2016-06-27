
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

using Owin;
using SimpleApiServerTest.OAuthProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SimpleApiServerTest.Configuration
{
    public static partial class Config
    {

        public static IAppBuilder UseOAuth(this IAppBuilder app, HttpConfiguration httpConfig)
        {
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
            });
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/oauth2/token"),
                Provider = new SPOAuthProvider(),
                AllowInsecureHttp = true,
                AccessTokenProvider = new AuthenticationTokenProvider(2),  
                RefreshTokenProvider=new AuthenticationTokenProvider(720),              
            });
            return app;
        }
    }
}