using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EasyHttpClient.Utilities;

namespace EasyHttpClient.DelegatingHandlers
{
    public class RetryHttpHandler : DelegatingHandler
    {
        private int _maxRetries;

        public RetryHttpHandler(int maxRetries)
        {
            this._maxRetries = maxRetries;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var maxRetries = Math.Max(_maxRetries, 0) + 1;
            Task<HttpResponseMessage> responseTask = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    responseTask = base.SendAsync(request, cancellationToken);
                    var response = await responseTask;
                    if ((int)response.StatusCode <= 500)
                    {
                        return response;
                    }
                }
                catch (HttpRequestException ex)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            return await responseTask;
        }
    }
}
