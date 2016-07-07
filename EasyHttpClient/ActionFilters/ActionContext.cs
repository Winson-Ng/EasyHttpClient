using EasyHttpClient.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.ActionFilters
{
    public class ActionContext
    {
        internal ActionContext(IMethodCallMessage methodCallMessage)
        {
            this.MethodCallMessage = methodCallMessage;
            this.Properties = new Dictionary<string, object>();
        }

        public IMethodCallMessage MethodCallMessage { get; private set; }

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

        public ParameterDescription[] Parameters
        {
            get { return this.MethodDescription.Parameters; }
        }
        public Dictionary<string, object> ParameterValues
        {
            get;
            internal set;
        }

        public IDictionary<string, object> Properties
        {
            get;
            private set;
        }

        public HttpRequestMessageBuilder HttpRequestMessageBuilder
        {
            get;
            internal set;
        }

        public IHttpResult CreateHttpResult()
        {

            return (IHttpResult)Activator.CreateInstance(typeof(HttpResult<>).MakeGenericType(this.MethodDescription.HttpResultObjectType), this.HttpClientSettings.JsonSerializerSettings);

        }
    }
}
