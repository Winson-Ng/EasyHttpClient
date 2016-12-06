using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using EasyHttpClient.Exceptions;

namespace EasyHttpClient.Utilities
{
    public static class HttpResultHelper
    {
        public static Task<IHttpResult<TofResult>> ToHttpResult<TofResult>(this HttpResponseMessage responseMessage)
        {
            return ToHttpResult<TofResult>(responseMessage, new JsonSerializerSettings());
        }

        public static async Task<IHttpResult<TofResult>> ToHttpResult<TofResult>(this HttpResponseMessage responseMessage, JsonSerializerSettings jsonSerializerSettings)
        {
            var result = await ToHttpResult(responseMessage, typeof(TofResult), new HttpClientSettings() { JsonSerializerSettings = jsonSerializerSettings });
            return result as IHttpResult<TofResult>;
        }

        internal static async Task<IHttpResult> ToHttpResult(this HttpResponseMessage responseMessage, Type returnObjectType, HttpClientSettings httpClientSettings)
        {

            IHttpResult httpResult = (IHttpResult)Activator.CreateInstance(typeof(HttpResult<>).MakeGenericType(returnObjectType), httpClientSettings.JsonSerializerSettings);
            httpResult.RequestMessage = responseMessage.RequestMessage;
            httpResult.Headers = responseMessage.Headers;
            httpResult.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
            httpResult.ReasonPhrase = responseMessage.ReasonPhrase;
            httpResult.StatusCode = responseMessage.StatusCode;
            httpResult.Version = responseMessage.Version;

            if (responseMessage.Content != null)
            {
                httpResult.ContentHeaders = responseMessage.Content.Headers;

                if (responseMessage.IsSuccessStatusCode)
                {
                    Exception exception = null;
                    try
                    {
                        if (returnObjectType.IsBulitInType())
                        {
                            httpResult.Content = ObjectExtensions.ChangeType(await responseMessage.Content.ReadAsStringAsync(), returnObjectType);
                            responseMessage.Dispose();
                        }
                        else if (typeof(Stream).IsAssignableFrom(returnObjectType))
                        {
                            httpResult.Content = await responseMessage.Content.ReadAsStreamAsync();
                        }
                        else if (typeof(byte[]) == returnObjectType)
                        {
                            httpResult.Content = await responseMessage.Content.ReadAsByteArrayAsync();
                            responseMessage.Dispose();
                        }
                        else
                        {
                            httpResult.Content = await responseMessage.Content.ReadAsAsync(returnObjectType, new[] { httpClientSettings.JsonMediaTypeFormatter });
                            responseMessage.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                    if (exception != null)
                    {
                        httpResult.ErrorMessage = await responseMessage.Content.ReadAsStringAsync();
                        responseMessage.Dispose();
                        throw new HttpResultException(httpResult, exception);
                    }
                }
                else
                {
                    httpResult.ErrorMessage = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.Dispose();
                }
            }
            return httpResult;
        }

        public static Task<IHttpResult> ToHttpResult(this Task<HttpResponseMessage> responseMessageTask, Type returnObjectType, HttpClientSettings httpClientSettings)
        {
            return responseMessageTask.Then(responseMessage => ToHttpResult(responseMessage, returnObjectType, httpClientSettings));
        }
    }
}
