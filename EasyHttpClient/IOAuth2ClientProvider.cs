using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
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

    public interface IOAuth2ClientProvider
    {
        /// <summary>
        /// Will be invoked by OAuth2Handler when oauth is required
        /// </summary>
        /// <param name="originalHttpRequestMessage"></param>
        Task SetAccessToken(HttpRequestMessage originalHttpRequestMessage);

        /// <summary>
        /// Will be invoked when http response 401. 
        /// </summary>
        /// <param name="originalHttpRequestMessage"></param>
        /// <returns></returns>
        Task<bool> RefreshAccessToken(HttpRequestMessage originalHttpRequestMessage);
    }

}
