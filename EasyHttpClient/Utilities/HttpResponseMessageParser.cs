using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace EasyHttpClient.Utilities
{
    public static class HttpResponseMessageParser
    {
        public static async Task<IHttpResult> ParseAsHttpResult(this HttpResponseMessage responseMessage, Type returnType, IEnumerable<MediaTypeFormatter> mediaTypeFormatters)
        {
            HttpResult httpResult = null;
            if (returnType.IsGenericType)
            {
                returnType = returnType.GenericTypeArguments[0];
                httpResult = (HttpResult)Activator.CreateInstance(typeof(HttpResult<>).MakeGenericType(returnType));
            }
            else
            {
                httpResult = new HttpResult();
            }

            httpResult.Headers = responseMessage.Headers;
            httpResult.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
            httpResult.ReasonPhrase = responseMessage.ReasonPhrase;
            //httpResult.RequestMessage = responseMessage.RequestMessage;
            httpResult.StatusCode = responseMessage.StatusCode;
            httpResult.Version = responseMessage.Version;

            if (responseMessage.IsSuccessStatusCode)
            {
                if (typeof(string) == returnType)
                {
                    httpResult.Content = await responseMessage.Content.ReadAsStringAsync();
                }
                else if (typeof(Stream) == returnType)
                {
                    httpResult.Content = await responseMessage.Content.ReadAsStreamAsync();
                }
                else if (typeof(byte[]) == returnType)
                {
                    httpResult.Content = await responseMessage.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    httpResult.Content = await responseMessage.Content.ReadAsAsync(returnType, mediaTypeFormatters);
                }
            }
            else
            {
                httpResult.ErrorMessage = await responseMessage.Content.ReadAsStringAsync();
            }
            return httpResult;
        }

        public static Task ParseAsVoid(this HttpResponseMessage responseMessage)
        {
            responseMessage.EnsureSuccessStatusCode();
            return Task.FromResult(0);
        }

        public static async Task<object> ParseAsObject(this HttpResponseMessage responseMessage, Type returnType, IEnumerable<MediaTypeFormatter> mediaTypeFormatters)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var objectType = returnType;

                if (objectType.IsBulitInType())
                {
                    var r = await responseMessage.Content.ReadAsStringAsync();
                    return Convert.ChangeType(r, objectType);
                }
                else if (typeof(Stream).IsAssignableFrom(returnType))
                {
                    return await responseMessage.Content.ReadAsStreamAsync();
                }
                else if (typeof(IEnumerable<byte>).IsAssignableFrom(returnType))
                {
                    return await responseMessage.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    return await responseMessage.Content.ReadAsAsync(objectType, mediaTypeFormatters);
                }
            }
            else
            {
                switch (responseMessage.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        break;
                    default:
                        responseMessage.EnsureSuccessStatusCode();
                        break;
                }
                return null;
            }
        }
    }
}
