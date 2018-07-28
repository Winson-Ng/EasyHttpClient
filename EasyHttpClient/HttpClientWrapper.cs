using EasyHttpClient.ActionFilters;
using EasyHttpClient.Attributes;
using EasyHttpClient.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using EasyHttpClient.Attributes.Parameter;

namespace EasyHttpClient
{
    public class HttpClientWrapper<T> : HttpClientWrapper
    {
        public HttpClientWrapper(
            HttpClient httpClient,
            Uri host,
            HttpClientSettings httpClientSettings)
            : base(typeof(T), httpClient, host, httpClientSettings)
        {
        }

        public new T GetTransparentProxy()
        {
            return (T)base.GetTransparentProxy();
        }
    }


    public class HttpClientWrapper : RealProxy
    {
        private readonly MethodInfo CastHttpResultTaskMethod = typeof(EasyHttpClient.Utilities.TaskExtensions).GetMethod("CastHttpResultTask");

        private readonly MethodInfo CastObjectTaskMethod = typeof(EasyHttpClient.Utilities.TaskExtensions).GetMethod("CastObjectTask");

        private static readonly MethodInfo[] _reservedMethods = typeof(object).GetMethods();
        /************

         * *************/
        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);
        private static readonly Regex RouteParameterParaser = new Regex(@"\{(?<paraName>[a-zA-Z_][a-zA-Z0-9_]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private RoutePrefixAttribute _routePrefixAttribute;
        //private JsonMediaTypeFormatter _jsonMediaTypeFormatter;
        private HttpClient _httpClient;
        private HttpClientSettings _httpClientSettings;
        private Uri _host;
        private bool _authorizeRequired;
        private Type _objectType;
        //private IHttpClientProvider _httpClientProvider { get; set; }


        public HttpClientWrapper(
            Type objectType,
            HttpClient httpClient,
            //IHttpClientProvider httpClientProvider, 
            Uri host,
            HttpClientSettings httpClientSettings)
            : base(objectType)
        {
            _httpClient = httpClient;
            _objectType = objectType;
            //_httpClientProvider = httpClientProvider;
            _host = host;
            _httpClientSettings = httpClientSettings;
            //_jsonMediaTypeFormatter = new JsonMediaTypeFormatter()
            //{
            //    SerializerSettings = _httpClientSettings.JsonSerializerSettings
            //};
            _routePrefixAttribute = objectType.GetCustomAttribute<RoutePrefixAttribute>();
            _authorizeRequired = objectType.IsDefined(typeof(AuthorizeAttribute));
        }

        const string MsgException = @"{0}: {1}";
        const string MsgMissingSomething = @"{0}: Missing {1}";
        const string MsgMissingSomethingOn = @"{0}: Missing {1} on {2}";

        private async Task<HttpResponseMessage> doSendHttpRequestAsync(HttpClient httpClient, ActionContext actionContext)
        {
            try
            {
                actionContext.HttpResponseMessage = await httpClient.SendAsync(actionContext.HttpRequestMessage, actionContext.CancellationToken.Token);
                return actionContext.HttpResponseMessage;
            }
            catch (OperationCanceledException ex)
            {
                actionContext.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    RequestMessage = actionContext.HttpRequestMessage,
                    ReasonPhrase = "RequestCanceled",
                    Content = new StringContent(ex.ToString())
                };
                return actionContext.HttpResponseMessage;
            }
            catch (HttpRequestException ex)
            {
                actionContext.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    RequestMessage = actionContext.HttpRequestMessage,
                    ReasonPhrase = "HttpRequestException",
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

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            try
            {
                if (_reservedMethods.Contains(methodCall.MethodBase))
                {
                    var unRegisteredReturn = methodCall.MethodName.Equals("GetType") ? _objectType : methodCall.MethodBase.Invoke(this, methodCall.Args);
                    var outArgs = methodCall.Args.Where(a => !methodCall.InArgs.Any(i => i == a)).ToArray();
                    return new ReturnMessage(unRegisteredReturn, outArgs, outArgs.Length, methodCall.LogicalCallContext, methodCall);
                }

                var actionContext = new ActionContext(methodCall);
                actionContext.HttpClientSettings = _httpClientSettings;
                //var _httpClient = this._httpClientProvider.GetClient();
                if (methodCall == null)
                {
                    throw new NotSupportedException(string.Format(MsgException, _objectType.ToString(), "Allow method call only!"));
                }
                var methodInfo = methodCall.MethodBase as MethodInfo;


                actionContext.MethodDescription = this.GetMethodDescription(methodInfo);

                var uriBuilder = new UriBuilder(Utility.CombinePaths(this._host
                    , _routePrefixAttribute != null ? (_routePrefixAttribute.Prefix + "/") : ""
                    , actionContext.MethodDescription.Route));

                actionContext.HttpRequestMessageBuilder = new HttpRequestMessageBuilder(actionContext.MethodDescription.HttpMethod, uriBuilder, _httpClientSettings);
                actionContext.HttpRequestMessageBuilder.MultiPartAttribute = actionContext.MethodDescription.MultiPartAttribute;

                actionContext.ParameterValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                if (methodCall.InArgCount > 0)
                {

                    for (var i = 0; i < methodCall.InArgCount; i++)
                    {
                        actionContext.ParameterValues[methodCall.GetInArgName(i)] = methodCall.GetInArg(i);
                    }

                    foreach (var p in actionContext.MethodDescription.Parameters)
                    {
                        object val;
                        if (actionContext.ParameterValues.TryGetValue(p.ParameterInfo.Name, out val))
                        {
                            foreach (var a in p.ScopeAttributes)
                            {
                                a.ProcessParameter(actionContext.HttpRequestMessageBuilder, p.ParameterInfo, val);
                            }
                        }
                    }
                }

                var httpResultTaskFunc = HttpRequestTaskFunc(actionContext, 0, () =>
                {

                    return actionContext.MethodDescription.HttpResultConverter(
                        EasyHttpClient.Utilities.TaskExtensions.Retry<HttpResponseMessage>(() =>
                        {
                            actionContext.HttpRequestMessage = actionContext.HttpRequestMessageBuilder.Build();

                            Task<HttpResponseMessage> httpRequestTask;
                            if (actionContext.MethodDescription.AuthorizeRequired && _httpClientSettings.OAuth2ClientHandler != null)
                            {
                                httpRequestTask = _httpClientSettings.OAuth2ClientHandler.SetAccessToken(actionContext.HttpRequestMessage)
                                    .Then(() => doSendHttpRequestAsync(_httpClient, actionContext))
                                    .Then(async response =>
                                    {
                                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                                        {
                                            actionContext.HttpRequestMessage = actionContext.HttpRequestMessageBuilder.Build();
                                            if (await _httpClientSettings.OAuth2ClientHandler.RefreshAccessToken(actionContext.HttpRequestMessage))
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

                return actionContext.MethodDescription.MethodResultConveter(methodCall, httpResultTaskFunc, actionContext);
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, methodCall);
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


        private static readonly Dictionary<MethodInfo, MethodDescription> MethodDescriptions = new Dictionary<MethodInfo, MethodDescription>();

        private MethodDescription GetMethodDescription(MethodInfo methodInfo)
        {
            MethodDescription methodDescription = null;

            if (!MethodDescriptions.TryGetValue(methodInfo, out methodDescription))
            {
                lock (MethodDescriptions)
                {
                    if (!MethodDescriptions.TryGetValue(methodInfo, out methodDescription))
                    {

                        var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();

                        if (routeAttribute == null)
                        {
                            throw new NotSupportedException(string.Format(MsgMissingSomethingOn, _objectType.ToString(), typeof(RouteAttribute).ToString(), methodInfo.ToString()));
                        }

                        var httpMethodAttribute = methodInfo.GetCustomAttributes().FirstOrDefault(t => t is IHttpMethodAttribute) as IHttpMethodAttribute;

                        if (httpMethodAttribute == null)
                        {
                            throw new NotSupportedException(string.Format(MsgMissingSomethingOn, _objectType.ToString(), typeof(IHttpMethodAttribute).ToString(), methodInfo.ToString()));
                        }

                        var httpMethod = httpMethodAttribute.HttpMethod;
                        var pathParamNames = ExtractParameterNames(routeAttribute.Path);
                        var parameters = methodInfo.GetParameters();
                        var attributedParameter = parameters
                            .Where(p => p.GetCustomAttributes().Any(a => a is IParameterScopeAttribute) && !p.IsDefined(typeof(HttpIgnoreAttribute)))
                            .Select(p => new ParameterDescription()
                            {
                                ParameterInfo = p,
                                ScopeAttributes = p.GetCustomAttributes().Where(a => a is IParameterScopeAttribute)
                                .Cast<IParameterScopeAttribute>().ToArray()
                            }).ToList();

                        var unhandledPathParamNames = pathParamNames.ToList();
                        foreach (var p in attributedParameter)
                        {
                            foreach (var a in p.ScopeAttributes)
                            {
                                if (a is PathParamAttribute)
                                {
                                    var name = a.Name ?? p.ParameterInfo.Name;
                                    if (unhandledPathParamNames.RemoveAll(i => string.Equals(i, name, StringComparison.OrdinalIgnoreCase)) > 0)
                                    {
                                        ((PathParamAttribute)a).PathParamNamesFilter = new string[] { name };
                                    }
                                    else
                                    {
                                        ((PathParamAttribute)a).PathParamNamesFilter = pathParamNames;
                                    }
                                }
                            }
                        }
                        var nonAttributedParameter = parameters.Where(p => !p.GetCustomAttributes().Any(a => a is IParameterScopeAttribute) && !p.IsDefined(typeof(HttpIgnoreAttribute)))
                            .Select(
                                p =>
                                {
                                    var attrList = new List<IParameterScopeAttribute>();
                                    if (unhandledPathParamNames.Any())
                                    {
                                        attrList.Add(new PathParamAttribute()
                                        {
                                            PathParamNamesFilter = unhandledPathParamNames.ToArray()
                                        });
                                    }

                                    attrList.Add(ParameterScopeAttributeFactory.CreateByHttpMethod(httpMethod, p));

                                    return new ParameterDescription()
                                    {
                                        ParameterInfo = p,
                                        ScopeAttributes = attrList.ToArray()
                                    };
                                }
                            ).ToList();

                        var httpRetryAttr = methodInfo.GetCustomAttribute<HttpRetryAttribute>();
                        methodDescription = new MethodDescription(methodInfo)
                        {
                            HttpMethod = httpMethod,
                            AuthorizeRequired = (_authorizeRequired ||
                                                methodInfo.IsDefined(typeof(AuthorizeAttribute)))
                                                && !methodInfo.IsDefined(typeof(AllowAnonymousAttribute)),
                            Route = routeAttribute.Path,
                            MaxRetry = httpRetryAttr != null ? httpRetryAttr.MaxRetry : _httpClientSettings.MaxRetry,
                            ActionFilters = _httpClientSettings.ActionFilters.Concat(methodInfo.GetCustomAttributes().Where(a => a is IActionFilter).Cast<IActionFilter>().OrderBy(i => i.Order)).ToArray(),
                            Parameters = attributedParameter.Union(nonAttributedParameter).ToArray(),
                            ReturnTypeDescription = new ReturnTypeDescription()
                        };

                        var returnType = methodInfo.ReturnType;
                        methodDescription.ReturnTypeDescription.ReturnType = methodInfo.ReturnType;
                        methodDescription.ReturnTypeDescription.TargetObjectType = methodInfo.ReturnType;
                        methodDescription.ReturnTypeDescription.HttpResultDecoder = methodInfo.GetCustomAttributes().FirstOrDefault(c => c is IHttpResultDecoder) as IHttpResultDecoder ?? _httpClientSettings.HttpResultDecoder;

                        if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType)
                        {
                            var taskObjectType = returnType.GenericTypeArguments[0];
                            if (typeof(IHttpResult).IsAssignableFrom(taskObjectType))
                            {
                                /************
                                 * Task<IHttpResult<TResult>>, 
                                 * Task<IHttpResult>
                                 * ***********/
                                if (taskObjectType.IsGenericType)
                                {
                                    methodDescription.ReturnTypeDescription.HttpResultType = taskObjectType.IsClass ? taskObjectType : typeof(HttpResult<>);
                                    methodDescription.ReturnTypeDescription.TargetObjectType = taskObjectType.GenericTypeArguments[0];
                                }
                                else
                                {
                                    methodDescription.ReturnTypeDescription.HttpResultType = taskObjectType.IsClass ? taskObjectType : typeof(HttpResult<>);
                                    methodDescription.ReturnTypeDescription.TargetObjectType = typeof(object);
                                }

                                methodDescription.MethodResultConveter = (methodCall, httpResultTask, actionContext) =>
                                {
                                    var result = CastHttpResultTaskMethod.MakeGenericMethod(taskObjectType).Invoke(null, new object[] { httpResultTask() });
                                    return new ReturnMessage(result, new object[0], 0, methodCall.LogicalCallContext, methodCall);
                                };
                            }
                            else
                            {
                                /*************
                                 * Task<TResult>
                                 * *************/
                                methodDescription.ReturnTypeDescription.TargetObjectType = returnType.GenericTypeArguments[0];

                                methodDescription.MethodResultConveter = (methodCall, httpResultTask, actionContext) =>
                                {
                                    var result = CastObjectTaskMethod.MakeGenericMethod(taskObjectType).Invoke(null, new object[] { httpResultTask() });
                                    return new ReturnMessage(result, new object[0], 0, methodCall.LogicalCallContext, methodCall);
                                };
                            }
                        }
                        else if (typeof(Task).IsAssignableFrom(returnType))
                        {
                            /*************
                             * Task
                             * *************/

                            var taskObjectType = typeof(void);
                            methodDescription.ReturnTypeDescription.TargetObjectType = typeof(object);

                            methodDescription.MethodResultConveter = (methodCall, httpResultTask, actionContext) =>
                            {
                                return new ReturnMessage(httpResultTask().Then(t =>
                                {
                                    if (!t.IsSuccessStatusCode)
                                    {
                                        throw new HttpException((int)t.StatusCode, t.ReasonPhrase);
                                    }
                                }), new object[0], 0, methodCall.LogicalCallContext, methodCall);
                            };
                        }
                        else if (typeof(IHttpResult).IsAssignableFrom(returnType))
                        {
                            /*************
                             * IHttpResult<TResult> 
                             * IHttpResult
                             * ************/

                            var taskObjectType = returnType;
                            if (returnType.IsGenericType)
                            {
                                methodDescription.ReturnTypeDescription.HttpResultType = returnType.IsClass ? returnType : typeof(HttpResult<>);
                                methodDescription.ReturnTypeDescription.TargetObjectType = returnType.GenericTypeArguments[0];
                            }
                            else
                            {
                                methodDescription.ReturnTypeDescription.HttpResultType = returnType.IsClass ? returnType : typeof(HttpResult<>);
                                methodDescription.ReturnTypeDescription.TargetObjectType = typeof(object);
                            }
                            methodDescription.MethodResultConveter = (methodCall, httpResultTask, actionContext) =>
                            {
                                var result = Task.Run(httpResultTask).Result;
                                return new ReturnMessage(ObjectExtensions.ChangeType(result, returnType), new object[0], 0, methodCall.LogicalCallContext, methodCall);
                            };
                        }
                        else if (returnType == typeof(void))
                        {
                            /*************
                             * void
                             * ***********/
                            var taskObjectType = typeof(void);
                            methodDescription.ReturnTypeDescription.TargetObjectType = typeof(object);

                            methodDescription.MethodResultConveter = (methodCall, httpResultTask, actionContext) =>
                            {
                                var t = Task.Run(httpResultTask).Result;
                                if (!t.IsSuccessStatusCode)
                                {
                                    throw new HttpException((int)t.StatusCode, t.ReasonPhrase);
                                }
                                return new ReturnMessage(null, new object[0], 0, methodCall.LogicalCallContext, methodCall);
                            };
                        }
                        else
                        {
                            /***********
                             * TResult
                             * object
                             * ************/

                            var taskObjectType = returnType;
                            methodDescription.ReturnTypeDescription.TargetObjectType = returnType;

                            methodDescription.MethodResultConveter = (methodCall, httpResultTask, actionContext) =>
                            {
                                var result = Task.Run(httpResultTask).Result.Content;
                                return new ReturnMessage(ObjectExtensions.ChangeType(result, returnType), new object[0], 0, methodCall.LogicalCallContext, methodCall);
                            };
                        }

                        methodDescription.HttpResultConverter = (httpTask, actionContext) =>
                        {
                            return httpTask.ToHttpResult(methodDescription.ReturnTypeDescription.TargetObjectType, actionContext);
                        };

                        methodDescription.MultiPartAttribute = methodInfo.GetCustomAttribute<MultiPartAttribute>();

                        MethodDescriptions.Add(methodInfo, methodDescription);
                    }
                }
            }

            return methodDescription;

        }

        private string[] ExtractParameterNames(string path)
        {
            var result = new List<string>();
            var matchs = RouteParameterParaser.Matches(path);
            for (var i = 0; i < matchs.Count; i++)
            {
                var g = matchs[i].Groups["paraName"];
                if (g != null && matchs[i].Groups["paraName"].Success)
                {
                    result.Add(g.Value);
                }
            }
            return result.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

    }
}
