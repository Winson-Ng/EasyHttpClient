using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public interface IHttpResult<T> : IHttpResult
    {
        new T Content { get;}
    }

    public interface IHttpResult
    {
        object Content { get; }
        string ErrorMessage { get; }
        HttpResponseHeaders Headers { get; }
        bool IsSuccessStatusCode { get; }
        string ReasonPhrase { get; }
        HttpStatusCode StatusCode { get; }
        Version Version { get; }
    }


    internal class HttpResult : IHttpResult
    {
        public HttpResponseHeaders Headers
        {
            get;
            set;
        }

        public bool IsSuccessStatusCode
        {
            get;
            set;
        }

        public string ReasonPhrase
        {
            get;
            set;
        }

        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        public Version Version
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public object Content
        {
            get;
            set;
        }
    }

    internal class HttpResult<T> : HttpResult, IHttpResult<T>
    {
        public new T Content
        {
            get
            {
                return (T)base.Content;
            }
             set
            {
                base.Content = value;
            }
        }
    }
}
