using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleApiServerTest.OAuthProvider
{
    public class AuthenticationTokenProvider: IAuthenticationTokenProvider
    {
        private readonly int _hours;
        public AuthenticationTokenProvider(int hours) {
            _hours = hours;
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(_hours);
            context.SetToken(context.SerializeTicket());
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(int.Parse(System.Configuration.ConfigurationManager.AppSettings["OAuth_Refresh_Token_TimeSpan"]));
            context.SetToken(context.SerializeTicket());
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

    }
}