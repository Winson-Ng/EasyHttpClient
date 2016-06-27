using EasyHttpClient;
using EasyHttpClientTest.ApiClients;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClientTest
{
    public class OAuth2ClientProvider : IOAuth2ClientProvider
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
        
        private readonly OAuthApiClient _oauthApiClient;

        public OAuth2ClientProvider(OAuthApiClient oauthApiClient)
        {
            _oauthApiClient = oauthApiClient;
        }

        public Task SetAccessToken(HttpRequestMessage originalHttpRequestMessage)
        {
            originalHttpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            return Task.FromResult(0);
        }

        public Task<bool> RefreshAccessToken(HttpRequestMessage originalHttpRequestMessage)
        {
            try
            {
                var result = this._oauthApiClient.RefreshToken("hello_client_id", "hello_client_sercret", this.RefreshToken);
                if (result != null)
                {
                    this.AccessToken = result.AccessToken;
                    this.RefreshToken = result.RefreshToken;
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        public void Login()
        {
            var result = _oauthApiClient.Login("hello_client_id", "hello_client_sercret", "developerABC", Guid.NewGuid().ToString());

            this.AccessToken = result.AccessToken;
            this.RefreshToken = result.RefreshToken;
        }
    }
}
