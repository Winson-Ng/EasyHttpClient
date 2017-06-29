﻿using EasyHttpClient.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{

    public class EmptyActionContext : ActionContext
    {
        public EmptyActionContext()
            : base(new EmptyMethodCallMessage())
        {
            this.MethodDescription = new MethodDescription(this.MethodDescription.MethodInfo);
        }
    }

    public class ActionContext
    {
        internal ActionContext(IMethodCallMessage methodCallMessage)
        {
            this.MethodCallMessage = methodCallMessage;
            this.Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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

        public ReturnTypeDescription ReturnTypeDescription
        {
            get { return this.MethodDescription.ReturnTypeDescription; }
        }

        public ParameterDescription[] Parameters
        {
            get { return this.MethodDescription.Parameters; }
        }
        public IDictionary<string, object> ParameterValues
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
            set;
        }

        /// <summary>
        /// Pls note, it won't run HttpRequestMessageBuilder.Build() if not null
        /// </summary>
        public HttpRequestMessage HttpRequestMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Pls note, it won't send out HTTP request if not null
        /// </summary>
        public HttpResponseMessage HttpResponseMessage
        {
            get;
            set;
        }


        public IHttpResult CreateHttpResult()
        {
            var httpResultType = MethodDescription.ReturnTypeDescription.HttpResultType;
            return (IHttpResult)Activator.CreateInstance(httpResultType.MakeGenericType(this.MethodDescription.ReturnTypeDescription.TargetObjectType), this.HttpClientSettings.JsonSerializerSettings);

        }

        public IHttpResult<T> CreateHttpResult<T>()
        {
            var httpResultType = MethodDescription.ReturnTypeDescription.HttpResultType;
            return (IHttpResult<T>)Activator.CreateInstance(httpResultType.MakeGenericType(typeof(T)), this.HttpClientSettings.JsonSerializerSettings);
        }
    }
}
