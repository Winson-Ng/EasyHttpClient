using EasyHttpClient;
using EasyHttpClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClientTest.ApiClients
{
    public interface OAuthApiClient
    {
        [Route("oauth2/token")]
        [HttpPost]
        TokenEndpointResponse Login(
            string client_id
            , string client_secret
            , string username
            , string password
            , string grant_type = "password");

        [Route("oauth2/token")]
        [HttpPost]
        TokenEndpointResponse RefreshToken(
            string client_id
            , string client_secret
            , string refresh_token
            , string grant_type = "refresh_token");
    }
}
