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
            var httpResult = (IHttpResult)Activator.CreateInstance(returnType);
            httpResult.Headers = responseMessage.Headers;
            httpResult.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
            httpResult.ReasonPhrase = responseMessage.ReasonPhrase;
            //httpResult.RequestMessage = responseMessage.RequestMessage;
            httpResult.StatusCode = responseMessage.StatusCode;
            httpResult.Version = responseMessage.Version;

            if (responseMessage.IsSuccessStatusCode)
            {
                if (returnType.IsGenericType
                    && returnType.GetGenericTypeDefinition() == typeof(HttpObjectResult<>))
                {
                    var objectType = returnType.GenericTypeArguments[0];
                    httpResult.Content = await responseMessage.Content.ReadAsAsync(objectType, mediaTypeFormatters);
                }
                else if (typeof(HttpStringResult) == returnType)
                {
                    httpResult.Content = await responseMessage.Content.ReadAsStringAsync();
                }
                else if (typeof(HttpStreamResult) == returnType)
                {
                    httpResult.Content = await responseMessage.Content.ReadAsStreamAsync();
                }
                else if (typeof(HttpBinaryResult) == returnType)
                {
                    httpResult.Content = await responseMessage.Content.ReadAsByteArrayAsync();
                }
            }
            else
            {
                httpResult.ErrorMessage = await responseMessage.Content.ReadAsStringAsync();
            }
            return httpResult;
        }

        public static async Task ParseAsVoid(this HttpResponseMessage responseMessage)
        {
            responseMessage.EnsureSuccessStatusCode();
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
