using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.DelegatingHandlers
{
    public class OAuth2Handler : DelegatingHandler
    {
        private readonly IOAuth2ClientProvider _oauth2ClientProvider;
        public OAuth2Handler(IOAuth2ClientProvider oauth2ClientProvider)
        {
            _oauth2ClientProvider = oauth2ClientProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Properties.ContainsKey("AuthorizeRequired"))
            {
                await this._oauth2ClientProvider.SetAccessToken(request);
                var response = await base.SendAsync(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (await this._oauth2ClientProvider.RefreshAccessToken(request))
                    {
                        response = await base.SendAsync(request, cancellationToken);
                    }
                    else {
                        throw new UnauthorizedAccessException("RefreshAccessToken fail");
                    }
                }
                return response;
            }
            else {
               return await base.SendAsync(request, cancellationToken);
            }

            //return base.SendAsync(request, cancellationToken);
        }
    }
}
