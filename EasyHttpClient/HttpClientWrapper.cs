using EasyHttpClient.Attributes;
using EasyHttpClient.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace EasyHttpClient
{
    internal class HttpClientWrapper<T> : RealProxy
    {
        private readonly MethodInfo CastTaskMethod = typeof(EasyHttpClient.Utilities.TaskExtensions).GetMethod("CastTask");

        /************

         * *************/
        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);
        private static readonly Regex RouteParameterParaser = new Regex(@"\{(?<paraName>[a-zA-Z_][a-zA-Z0-9_]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private RoutePrefixAttribute _routePrefixAttribute;
        private JsonMediaTypeFormatter _jsonMediaTypeFormatter;
        private HttpClient _httpClient;
        private HttpClientSettings _httpClientSettings;
        private Uri _host;
        private bool _authorizeRequired;
        //private IHttpClientProvider _httpClientProvider { get; set; }

        internal HttpClientWrapper(
            HttpClient httpClient,
            //IHttpClientProvider httpClientProvider, 
            Uri host,
            HttpClientSettings httpClientSettings)
            : base(typeof(T))
        {
            var objectType = typeof(T);
            _httpClient = httpClient;
            //_httpClientProvider = httpClientProvider;
            _host = host;
            _httpClientSettings = httpClientSettings;
            _jsonMediaTypeFormatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = _httpClientSettings.JsonSerializerSettings
            };
            _routePrefixAttribute = objectType.GetCustomAttribute<RoutePrefixAttribute>();
            _authorizeRequired = objectType.IsDefined(typeof(AuthorizeAttribute));
        }

        const string MsgException = @"{0}: {1}";
        const string MsgMissingSomething = @"{0}: Missing {1}";
        const string MsgMissingSomethingOn = @"{0}: Missing {1} on {2}";

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;

            try
            {
                //var _httpClient = this._httpClientProvider.GetClient();
                if (methodCall == null)
                {
                    throw new NotSupportedException(string.Format(MsgException, typeof(T).ToString(), "Allow method call only!"));
                }
                var methodInfo = methodCall.MethodBase as MethodInfo;

                //if (_routePrefixAttribute == null)
                //{
                //    throw new NotSupportedException(string.Format(MsgException, typeof(T).ToString(), typeof(RoutePrefixAttribute).ToString()));
                //}

                var parameters = this.GetMethodParameters(methodInfo);

                var uriBuilder = new UriBuilder(Utility.BuildPath(this._host
                    , _routePrefixAttribute != null ? (_routePrefixAttribute.Prefix + "/") : ""
                    , parameters.Route));

                HttpRequestMessage httpMessage = null; //new HttpRequestMessage(parameters.HttpMethod, uriBuilder.Uri);

                if (methodCall.InArgCount > 0)
                {
                    var httpRequestMessageBuilder = new HttpRequestMessageBuilder(parameters.HttpMethod, uriBuilder, _httpClientSettings.JsonSerializerSettings);

                    var tempParamValueDict = new Dictionary<string, object>();

                    for (var i = 0; i < methodCall.InArgCount; i++)
                    {
                        tempParamValueDict[methodCall.GetInArgName(i)] = methodCall.GetInArg(i);
                    }

                    foreach (var p in parameters.ParameterInfos)
                    {
                        object val;
                        if (tempParamValueDict.TryGetValue(p.ParameterInfo.Name, out val))
                        {
                            foreach (var a in p.ParameterAttributes)
                            {
                                a.ProcessParameter(httpRequestMessageBuilder, p.ParameterInfo, val);
                            }
                        }
                    }
                    httpMessage = httpRequestMessageBuilder.Build();
                }
                else
                {
                    httpMessage = new HttpRequestMessage(parameters.HttpMethod, uriBuilder.Uri);
                }

                if (parameters.AuthorizeRequired)
                {
                    httpMessage.Properties["AuthorizeRequired"] = true;
                }
                else
                {
                    httpMessage.Properties.Remove("AuthorizeRequired");
                }

                var httpRequestTask = _httpClient.SendAsync(httpMessage); //AutoRetryRequest(_httpClient, httpMessage, _httpClientSettings.AutoRetryLimit);

                var returnType = methodInfo.ReturnType;
                if (returnType.IsGenericType && returnType.IsSubclassOf(typeof(Task)))
                {
                    returnType = returnType.GenericTypeArguments[0];

                    var task = ParseResult(httpRequestTask, returnType);
                    var result = CastTaskMethod.MakeGenericMethod(returnType).Invoke(null, new object[] { task });
                    return new ReturnMessage(result, new object[0], 0, methodCall.LogicalCallContext, methodCall);
                }
                else if (returnType == typeof(Task))
                {
                    return new ReturnMessage(Task.Run(() => httpRequestTask.ContinueWith(async t =>
                    {
                        (await t).EnsureSuccessStatusCode();
                    }).Unwrap()), new object[0], 0, methodCall.LogicalCallContext, methodCall);

                }
                else if (returnType == typeof(void))
                {
                    var result = Task.Run(() => httpRequestTask).Result;
                    return new ReturnMessage(result.EnsureSuccessStatusCode(), new object[0], 0, methodCall.LogicalCallContext, methodCall);
                }
                else
                {
                    var result = Task.Run(() => ParseResult(httpRequestTask, returnType)).Result;
                    return new ReturnMessage(result, new object[0], 0, methodCall.LogicalCallContext, methodCall);
                }
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, methodCall);
            }
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

        private async Task<object> ParseResult(Task<HttpResponseMessage> requestTask, Type returnType)
        {
            var ret = await requestTask.ContinueWith(async t =>
            {
                var responseMessage = await t;
                if (typeof(IHttpResult).IsAssignableFrom(returnType))
                {
                    return await responseMessage.ParseAsHttpResult(returnType, new[] { _jsonMediaTypeFormatter });
                }
                else
                {
                    return await responseMessage.ParseAsObject(returnType, new[] { _jsonMediaTypeFormatter });
                }
            }).Unwrap();
            return ret;
        }

        private static readonly Dictionary<MethodInfo, RequestMethodParameters> MethodRequestParameterAttributeDict = new Dictionary<MethodInfo, RequestMethodParameters>();

        private RequestMethodParameters GetMethodParameters(MethodInfo methodInfo)
        {
            RequestMethodParameters result = null;

            if (!MethodRequestParameterAttributeDict.TryGetValue(methodInfo, out result))
            {
                lock (MethodRequestParameterAttributeDict)
                {
                    if (!MethodRequestParameterAttributeDict.TryGetValue(methodInfo, out result))
                    {

                        var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();

                        if (routeAttribute == null)
                        {
                            throw new NotSupportedException(string.Format(MsgMissingSomethingOn, typeof(T).ToString(), typeof(RouteAttribute).ToString(), methodInfo.ToString()));
                        }

                        var httpMethodAttribute = methodInfo.GetCustomAttributes().FirstOrDefault(t => t is IHttpMethodAttribute) as IHttpMethodAttribute;

                        if (httpMethodAttribute == null)
                        {
                            throw new NotSupportedException(string.Format(MsgMissingSomethingOn, typeof(T).ToString(), typeof(IHttpMethodAttribute).ToString(), methodInfo.ToString()));
                        }

                        var httpMethod = httpMethodAttribute.HttpMethod;
                        var pathParamNames = ExtractParameterNames(routeAttribute.Path);
                        var parameters = methodInfo.GetParameters();
                        var attributedParameter = parameters
                            .Where(p => p.GetCustomAttributes().Any(a => a is IParameterAttribute))
                            .Select(p => new ParameterWithAttributes()
                            {
                                ParameterInfo = p,
                                ParameterAttributes = p.GetCustomAttributes().Where(a => a is IParameterAttribute)
                                .Cast<IParameterAttribute>().ToArray()
                            });
                        var nonAttributedParameter = parameters.Where(p => !p.GetCustomAttributes().Any(a => a is IParameterAttribute)).Select(
                                p =>
                                {
                                    var attrList = new List<IParameterAttribute>();

                                    if (pathParamNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                                    {
                                        attrList.Add(new PathParamAttribute()
                                        {
                                            Name = p.Name
                                        });
                                    }
                                    attrList.Add(ParameterAttributeFactory.CreateByHttpMethod(httpMethod, p));

                                    return new ParameterWithAttributes()
                                    {
                                        ParameterInfo = p,
                                        ParameterAttributes = attrList.ToArray()
                                    };
                                }
                            );
                        result = new RequestMethodParameters()
                        {
                            HttpMethod = httpMethod,
                            AuthorizeRequired = (_authorizeRequired ||
                            methodInfo.IsDefined(typeof(AuthorizeAttribute)))
                            && !methodInfo.IsDefined(typeof(AllowAnonymousAttribute)),
                            Route = routeAttribute.Path,
                            ParameterInfos = attributedParameter.Union(nonAttributedParameter).ToArray()
                        };

                        MethodRequestParameterAttributeDict.Add(methodInfo, result);
                    }
                }
            }

            return result;

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
            return result.Distinct().ToArray();
        }

    }
}
