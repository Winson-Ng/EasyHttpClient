using EasyHttpClient.Attributes;
using EasyHttpClient.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Utilities
{
    public sealed class EasyHttpRequest
    {
        public static EasyHttpRequest<T> Create<T>(HttpMethod httpMethod, Uri url)
        {
            return new EasyHttpRequest<T>(httpMethod, url);
        }

        public static EasyHttpRequest<T> Create<T>(HttpMethod httpMethod, string url)
        {
            return new EasyHttpRequest<T>(httpMethod, new Uri(url));
        }

        public static EasyHttpRequest<T> Get<T>(Uri url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Get, url);
        }
        public static EasyHttpRequest<T> Get<T>(string url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Get, new Uri(url));
        }

        public static EasyHttpRequest<T> Post<T>(Uri url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Post, url);
        }
        public static EasyHttpRequest<T> Post<T>(string url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Post, new Uri(url));
        }

        public static EasyHttpRequest<T> Put<T>(Uri url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Put, url);
        }
        public static EasyHttpRequest<T> Put<T>(string url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Put, new Uri(url));
        }

        public static EasyHttpRequest<T> Delete<T>(Uri url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Delete, url);
        }
        public static EasyHttpRequest<T> Delete<T>(string url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Delete, new Uri(url));
        }

        public static EasyHttpRequest<T> Head<T>(Uri url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Head, url);
        }
        public static EasyHttpRequest<T> Head<T>(string url)
        {
            return new EasyHttpRequest<T>(HttpMethod.Head, new Uri(url));
        }

    }

    public class EasyHttpRequest<T>
    {
        private HttpRequestMessageBuilder _httpRequestMessageBuilder;
        private bool _implementOauth;

        public IHttpClientProvider HttpClientProvider { get; set; }
        public HttpClientSettings HttpClientSettings { get; set; }
        public EasyHttpRequest(HttpMethod httpMethod, Uri url)
        {
            this.HttpClientSettings = new HttpClientSettings();
            this.HttpClientProvider = new DefaultHttpClientProvider();
            this._httpRequestMessageBuilder = new HttpRequestMessageBuilder(httpMethod, new UriBuilder(url), this.HttpClientSettings);
        }

        public EasyHttpRequest<T> SetOAuth(IOAuth2ClientHandler oauthHandler)
        {
            this._implementOauth = oauthHandler != null;
            this.HttpClientSettings.OAuth2ClientHandler = oauthHandler;
            return this;
        }
        public EasyHttpRequest<T> SetOAuth(bool implementOauth)
        {
            this._implementOauth = implementOauth;
            return this;
        }
        public EasyHttpRequest<T> AddPathParam(string name, object value)
        {
            if (value != null)
            {
                var pa = new PathParamAttribute();
                pa.PathParamNamesFilter = new[] { name };
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddPathParams(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new PathParamAttribute();
                    pa.PathParamNamesFilter = new string[0];
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }


        public EasyHttpRequest<T> AddQueryString(string name, object value)
        {
            if (value != null)
            {
                var pa = new QueryStringAttribute();
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddQueryStrings(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new QueryStringAttribute();
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }

        public EasyHttpRequest<T> AddFormBodyParam(string name, object value)
        {
            if (value != null)
            {
                var pa = new FormBodyAttribute();
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddFormBodyParams(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new QueryStringAttribute();
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }

        public EasyHttpRequest<T> AddJsonBodyParam(string name, object value, JTokenType jtokenType = JTokenType.JObject)
        {
            if (value != null)
            {
                var pa = new JsonBodyAttribute()
                {
                    Name = name,
                    JTokenType = jtokenType
                };
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddJsonBodyParams(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new JsonBodyAttribute()
                    {
                        Name = string.Empty
                    };
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }

        public EasyHttpRequest<T> AddJsonBodyParams(JTokenType jtokenType = JTokenType.JObject, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new JsonBodyAttribute()
                    {
                        Name = string.Empty,
                        JTokenType = jtokenType
                    };
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }

        public EasyHttpRequest<T> AddHeader(string name, object value)
        {
            if (value != null)
            {
                var pa = new HeaderAttribute();
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddHeaders(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new HeaderAttribute();
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }


        public EasyHttpRequest<T> AddCookie(string name, object value)
        {
            if (value != null)
            {
                var pa = new CookieAttribute();
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddCookies(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new HeaderAttribute();
                    pa.ProcessParameter(_httpRequestMessageBuilder, p);
                }
            }
            return this;
        }

        public EasyHttpRequest<T> AddFile(string name, object value)
        {
            if (value != null)
            {
                var pa = new FileContentAttribute();
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, null, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddFiles(string name, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new FileContentAttribute()
                    {
                        Name = name
                    };
                    pa.ProcessParameter(_httpRequestMessageBuilder, null, p);
                }
            }
            return this;
        }

        public EasyHttpRequest<T> AddRawContent(string name, object value)
        {
            if (value != null)
            {
                var pa = new RawContentAttribute();
                pa.Name = name;
                pa.ProcessParameter(_httpRequestMessageBuilder, null, value);
            }
            return this;
        }

        public EasyHttpRequest<T> AddRawContents(string name, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var pa = new RawContentAttribute()
                    {
                        Name = name
                    };
                    pa.ProcessParameter(_httpRequestMessageBuilder, null, p);
                }
            }
            return this;
        }

        //public EasyHttpClientBuilder<T> SetHttpResultDecoder(IHttpResultDecoder decoder)
        //{
        //    return this;
        //}


        private async Task<HttpResponseMessage> doSendHttpRequestAsync(HttpClient httpClient, ActionContext actionContext)
        {
            try
            {
                actionContext.HttpResponseMessage = await httpClient.SendAsync(actionContext.HttpRequestMessage);
                return actionContext.HttpResponseMessage;
            }
            catch (HttpRequestException ex)
            {
                actionContext.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    RequestMessage = actionContext.HttpRequestMessage,
                    ReasonPhrase = "RequestTimeout",
                    Content = new StringContent(ex.ToString())
                };
                return actionContext.HttpResponseMessage;
            }
            catch (WebException we)
            {
                switch (we.Status)
                {
                    case WebExceptionStatus.Timeout:
                        actionContext.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                        {
                            RequestMessage = actionContext.HttpRequestMessage,
                            ReasonPhrase = HttpStatusCode.RequestTimeout.ToString(),
                            Content = new StringContent(we.ToString())
                        };
                        return actionContext.HttpResponseMessage;
                    case WebExceptionStatus.ConnectFailure:
                    case WebExceptionStatus.ConnectionClosed:
                    case WebExceptionStatus.PipelineFailure:
                        actionContext.HttpResponseMessage = new HttpResponseMessage((HttpStatusCode)599)
                        {
                            RequestMessage = actionContext.HttpRequestMessage,
                            ReasonPhrase = we.Status.ToString(),
                            Content = new StringContent(we.ToString())
                        };
                        return actionContext.HttpResponseMessage;
                    //case WebExceptionStatus.NameResolutionFailure:
                    //case WebExceptionStatus.ProxyNameResolutionFailure:
                    //    return new HttpResponseMessage((HttpStatusCode)523)
                    //    {
                    //        ReasonPhrase = we.Status.ToString(),
                    //        Content = new StringContent(we.ToString())
                    //    };
                    case WebExceptionStatus.SecureChannelFailure:
                    case WebExceptionStatus.TrustFailure:
                        actionContext.HttpResponseMessage = new HttpResponseMessage((HttpStatusCode)525)
                        {
                            RequestMessage = actionContext.HttpRequestMessage,
                            ReasonPhrase = we.Status.ToString(),
                            Content = new StringContent(we.ToString())
                        };
                        return actionContext.HttpResponseMessage;
                    default:
                        throw;
                }
            }
        }


        private Func<Task<IHttpResult>> HttpRequestTaskFunc(ActionContext context, int index, Func<Task<IHttpResult>> continuation)
        {
            var a = context.MethodDescription.ActionFilters.ElementAtOrDefault(index);
            if (a != null)
            {
                return () => a.ActionInvoke(context, HttpRequestTaskFunc(context, index + 1, continuation));
            }
            else
            {
                return continuation;
            }
        }

        public async Task<IHttpResult<T>> SendAsync()
        {
            var methodCall = new EmptyMethodCallMessage();
            var actionContext = new ActionContext(methodCall)
            {
                HttpClientSettings = this.HttpClientSettings,
                MethodDescription = new MethodDescription()
                {
                    ActionFilters = this.HttpClientSettings.ActionFilters.ToArray(),
                    AuthorizeRequired = false,
                    HttpMethod = this._httpRequestMessageBuilder.HttpMethod,
                    ReturnTypeDescription = new ReturnTypeDescription()
                            {
                                HttpResultDecoder = this.HttpClientSettings.HttpResultDecoder,
                                ReturnType = typeof(IHttpResult<T>),
                                TargetObjectType = typeof(T),
                            },
                    HttpResultConverter = (httpTask, ctx) =>
                            {
                                return httpTask.ToHttpResult(typeof(T), ctx);
                            },
                    MaxRetry = this.HttpClientSettings.MaxRetry,
                }
            };
            var _httpClient = this.HttpClientProvider.GetClient(this.HttpClientSettings);
            var httpResultTaskFunc = HttpRequestTaskFunc(actionContext, 0, () =>
            {

                return actionContext.MethodDescription.HttpResultConverter(
                    EasyHttpClient.Utilities.TaskExtensions.Retry<HttpResponseMessage>(() =>
                    {
                        actionContext.HttpRequestMessage = this._httpRequestMessageBuilder.Build();

                        Task<HttpResponseMessage> httpRequestTask;
                        if (actionContext.MethodDescription.AuthorizeRequired && this.HttpClientSettings.OAuth2ClientHandler != null)
                        {
                            httpRequestTask = this.HttpClientSettings.OAuth2ClientHandler.SetAccessToken(actionContext.HttpRequestMessage)
                                .Then(() => doSendHttpRequestAsync(_httpClient, actionContext))
                                .Then(async response =>
                                {
                                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                                    {
                                        actionContext.HttpRequestMessage = actionContext.HttpRequestMessageBuilder.Build();
                                        if (await this.HttpClientSettings.OAuth2ClientHandler.RefreshAccessToken(actionContext.HttpRequestMessage))
                                        {
                                            response = await doSendHttpRequestAsync(_httpClient, actionContext);
                                        }
                                    }
                                    return response;
                                });
                        }
                        else
                        {
                            httpRequestTask = doSendHttpRequestAsync(_httpClient, actionContext);
                        }
                        return httpRequestTask;
                    }, (r) => Task.FromResult((int)r.StatusCode > 500 || r.StatusCode == HttpStatusCode.RequestTimeout), actionContext.MethodDescription.MaxRetry)
                    , actionContext);

            });

            var result = await httpResultTaskFunc() as IHttpResult<T>;
            return result;
        }

        public IHttpResult<T> Send()
        {
            throw new NotImplementedException();
        }
    }
}
