using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EasyHttpClient.Utilities;

namespace EasyHttpClient.DelegatingHandlers
{
    public class RetryHandler : DelegatingHandler
    {
        private int _maxRetries;

        public RetryHandler(int maxRetries)
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


        public Task<HttpResponseMessage> AutoRetryRequest(HttpClient httpClient, HttpRequestMessage httpMessage, int retryLimit)
        {
            return httpClient.SendAsync(httpMessage).ContinueWith(async t =>
            {
                if (retryLimit-- > 0)
                {
                    var toRetry = false;
                    if (t.IsFaulted)
                    {
                        if (t.Exception.InnerExceptions != null)
                        {
                            foreach (var e in t.Exception.InnerExceptions)
                            {
                                if (e is HttpRequestException)
                                {
                                    toRetry = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var r = await t;
                        if ((int)r.StatusCode > 500)
                        {
                            toRetry = true;
                        }
                    }

                    if (toRetry)
                    {
                        return await AutoRetryRequest(httpClient, httpMessage.Clone(), retryLimit);
                    }
                }
                return await t;
            }).Unwrap();
        }
    }
}
