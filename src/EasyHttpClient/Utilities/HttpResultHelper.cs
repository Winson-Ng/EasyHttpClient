using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using EasyHttpClient.Exceptions;
using EasyHttpClient.Attributes;

namespace EasyHttpClient.Utilities
{
    public static class HttpResultHelper
    {
        private static readonly IHttpResultDecoder DefaultHttpResultDecoder = new JsonResultDecoderAttribute();
        public static Task<IHttpResult<TofResult>> ToHttpResult<TofResult>(this HttpResponseMessage responseMessage)
        {
            return ToHttpResult<TofResult>(responseMessage, new JsonSerializerSettings());
        }

        public static async Task<IHttpResult<TofResult>> ToHttpResult<TofResult>(this HttpResponseMessage responseMessage, JsonSerializerSettings jsonSerializerSettings)
        {
            var actionContext = new EmptyActionContext();
            actionContext.HttpClientSettings = new HttpClientSettings() { JsonSerializerSettings = jsonSerializerSettings };
            actionContext.ReturnTypeDescription.ReturnType = typeof(IHttpResult<TofResult>);
            actionContext.ReturnTypeDescription.TargetObjectType = typeof(TofResult);

            var result = await ToHttpResult(responseMessage, typeof(TofResult), actionContext);
            return result as IHttpResult<TofResult>;
        }

        internal static async Task<IHttpResult> ToHttpResult(this HttpResponseMessage responseMessage, Type returnObjectType, ActionContext actionContext)
        {
            var httpClientSettings = actionContext.HttpClientSettings;
            var httpResult = actionContext.CreateHttpResult();
            httpResult.Headers = responseMessage.Headers;
            httpResult.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
            httpResult.ReasonPhrase = responseMessage.ReasonPhrase;
            httpResult.StatusCode = responseMessage.StatusCode;
            httpResult.Version = responseMessage.Version;

            if (responseMessage.Content != null)
            {
                httpResult.ContentHeaders = responseMessage.Content.Headers;

                if (actionContext.ReturnTypeDescription.HttpResultDecoder!=null && actionContext.ReturnTypeDescription.HttpResultDecoder.CanDecode(responseMessage.Content.Headers))
                {
                    httpResult.Content = await actionContext.ReturnTypeDescription.HttpResultDecoder.DecodeAsync(responseMessage.Content, actionContext);
                }
                else {

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
                                httpResult.Content = await DefaultHttpResultDecoder.DecodeAsync(responseMessage.Content, actionContext);                                
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
            }
            return httpResult;
        }



        public static Task<IHttpResult> ToHttpResult(this Task<HttpResponseMessage> responseMessageTask, Type returnObjectType, ActionContext actionContext)
        {
            return responseMessageTask.Then(responseMessage => ToHttpResult(responseMessage, returnObjectType, actionContext));
        }

        public static Task<IHttpResult> ToHttpResult(this Task<HttpResponseMessage> responseMessageTask, Type returnObjectType, HttpClientSettings httpClientSettings)
        {
            var actionContext = new EmptyActionContext();
            actionContext.HttpClientSettings = httpClientSettings;
            actionContext.ReturnTypeDescription.ReturnType = typeof(IHttpResult);
            actionContext.ReturnTypeDescription.TargetObjectType = returnObjectType;

            return responseMessageTask.Then(responseMessage => ToHttpResult(responseMessage, returnObjectType, actionContext));
        }
    }
}
