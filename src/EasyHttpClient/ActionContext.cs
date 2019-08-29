using EasyHttpClient.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyHttpClient
{

    public class EmptyActionContext : ActionContext
    {
        public EmptyActionContext()
        {
            this.MethodDescription = new MethodDescription(this.MethodDescription.MethodInfo);
        }
    }

    public class ActionContext
    {
        public CancellationTokenSource CancellationToken { get; private set; }

        internal ActionContext()
        {
            this.Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.CancellationToken = new CancellationTokenSource();
        }

        public HttpClientSettings HttpClientSettings
        {
            get;
            internal set;
        }
        public MethodDescription MethodDescription
        {
            get;
            internal set;
        }

        public ReturnTypeDescription ReturnTypeDescription
        {
            get { return this.MethodDescription.ReturnTypeDescription; }
        }

        public ParameterDescription[] Parameters
        {
            get { return this.MethodDescription.Parameters; }
        }

        public IDictionary<string, object> Properties
        {
            get;
            private set;
        }

        public HttpRequestMessageBuilder HttpRequestMessageBuilder
        {
            get;
            set;
        }

        /// <summary>
        /// Pls note, it won't run HttpRequestMessageBuilder.Build() if not null
        /// </summary>
        public HttpRequestMessage HttpRequestMessage
        {
            get;
            internal set;
        }
        /// <summary>
        /// Pls note, it won't send out HTTP request if not null
        /// </summary>
        public HttpResponseMessage HttpResponseMessage
        {
            get;
            internal set;
        }


        public IHttpResult CreateHttpResult()
        {
            var httpResultType = MethodDescription.ReturnTypeDescription.HttpResultType;
            IHttpResult httpResult = null;
            if (httpResultType == typeof(HttpResult<>))
            {
                httpResult = (IHttpResult)Activator.CreateInstance(typeof(HttpResult<>).MakeGenericType(this.MethodDescription.ReturnTypeDescription.TargetObjectType), this.HttpClientSettings.JsonSerializerSettings);
            }
            else
            {
                var constructors = httpResultType.GetConstructors();
                if (constructors.Any(w => w.GetParameters()
                                              .Count(p => p.ParameterType == typeof(JsonSerializerSettings)) == 1))
                {
                    httpResult =
                        (IHttpResult)Activator.CreateInstance(httpResultType,
                            this.HttpClientSettings.JsonSerializerSettings);
                }
                else
                {
                    httpResult = (IHttpResult)Activator.CreateInstance(httpResultType);
                }

            }
            httpResult.RequestMessage = this.HttpRequestMessage;
            return httpResult;
        }

        public IHttpResult<T> CreateHttpResult<T>()
        {
            var httpResultType = MethodDescription.ReturnTypeDescription.HttpResultType;
            var result = (IHttpResult<T>)Activator.CreateInstance(httpResultType.MakeGenericType(typeof(T)), this.HttpClientSettings.JsonSerializerSettings);
            result.RequestMessage = this.HttpRequestMessage;

            IHttpResult<T> httpResult = null;
            if (httpResultType == typeof(HttpResult<>))
            {
                httpResult =new HttpResult<T>(this.HttpClientSettings.JsonSerializerSettings);
            }
            else
            {
                var constructors = httpResultType.GetConstructors();
                if (constructors.Any(w => w.GetParameters()
                                              .Count(p => p.ParameterType == typeof(JsonSerializerSettings)) == 1))
                {
                    httpResult =
                        (IHttpResult<T>)Activator.CreateInstance(httpResultType,
                            this.HttpClientSettings.JsonSerializerSettings);
                }
                else
                {
                    httpResult = (IHttpResult<T>)Activator.CreateInstance(httpResultType);
                }
            }
            httpResult.RequestMessage = this.HttpRequestMessage;
            return httpResult;


        }
    }
}
