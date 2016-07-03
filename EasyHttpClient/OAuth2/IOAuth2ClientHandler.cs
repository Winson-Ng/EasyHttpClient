using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.OAuth2
{
    public interface IOAuth2ClientHandler
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
