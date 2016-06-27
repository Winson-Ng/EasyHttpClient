using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SimpleApiServerTest.OAuthProvider
{
    public class SPOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var currentIdentity = context.Request.User.Identity as ClaimsIdentity;
            var properties = new AuthenticationProperties();
            var ticket = new AuthenticationTicket(currentIdentity, properties);
            context.Validated(ticket);
            return Task.FromResult(0);
        }
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            var properties = new AuthenticationProperties();
            var ticket = new AuthenticationTicket(identity, properties);
            context.Validated(ticket);
            return Task.FromResult(0);
        }
    }
}