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
    public class TokenEndpointRequest : Dictionary<string, object>
    {
        public new object this[string key]
        {
            get
            {
                object val;
                if (base.TryGetValue(key, out val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                base[key] = value;
            }
        }

        public string GrantType
        {
            get
            {
                return Convert.ToString(this["grant_type"]);
            }
            set
            {
                this["grant_type"] = value;
            }
        }

        public string RefreshToken
        {
            get
            {
                return Convert.ToString(this["refresh_token"]);
            }
            set
            {
                this["refresh_token"] = value;
            }
        }

        public string ClientId
        {
            get
            {
                return Convert.ToString(this["client_id"]);
            }
            set
            {
                this["client_id"] = value;
            }
        }

        public string ClientSecret
        {
            get
            {
                return Convert.ToString(this["client_secret"]);
            }
            set
            {
                this["client_secret"] = value;
            }
        }
    }

    public class TokenEndpointResponse : Dictionary<string, object>
    {
        public new object this[string key]
        {
            get
            {
                object val;
                if (base.TryGetValue(key, out val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                base[key] = value;
            }
        }

        public string AccessToken
        {
            get
            {
                return Convert.ToString(this["access_token"]);
            }
            set
            {
                this["access_token"] = value;
            }
        }

        public string TokenType
        {
            get
            {
                return Convert.ToString(this["token_type"]);
            }
            set
            {
                this["token_type"] = value;
            }
        }

        public long ExpiresIn
        {
            get
            {
                return Convert.ToInt64(this["expires_in"]);
            }
            set
            {
                this["expires_in"] = value;
            }
        }

        public string RefreshToken
        {
            get
            {
                return Convert.ToString(this["refresh_token"]);
            }
            set
            {
                this["refresh_token"] = value;
            }
        }
    }

    public class OAuth2ClientHandler : IOAuth2ClientHandler
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
        
        private readonly OAuthApiClient _oauthApiClient;

        public OAuth2ClientHandler(OAuthApiClient oauthApiClient)
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
