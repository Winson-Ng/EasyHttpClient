using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace EasyHttpClient.Utilities
{
    public static class HttpResponseMessageParser
    {
        public static Task<IHttpResult> ParseAsHttpResult(this Task<HttpResponseMessage> responseMessageTask, Type returnObjectType, HttpClientSettings httpClientSettings)
        {
            return responseMessageTask.Then(async responseMessage =>
            {
                IHttpResult httpResult = (IHttpResult)Activator.CreateInstance(typeof(HttpResult<>).MakeGenericType(returnObjectType), httpClientSettings.JsonSerializerSettings);

                httpResult.Headers = responseMessage.Headers;
                httpResult.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
                httpResult.ReasonPhrase = responseMessage.ReasonPhrase;
                httpResult.StatusCode = responseMessage.StatusCode;
                httpResult.Version = responseMessage.Version;

                if (responseMessage.Content != null)
                    httpResult.ContentHeaders = responseMessage.Content.Headers;

                if (responseMessage.IsSuccessStatusCode)
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
                else
                {
                    httpResult.ErrorMessage = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.Dispose();
                }
                return httpResult;
            });

        }
    }
}
