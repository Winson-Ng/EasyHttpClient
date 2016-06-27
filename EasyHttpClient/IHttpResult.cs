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
    public interface IHttpResult
    {
        string ErrorMessage { get; set; }
        // Summary:
        //     Gets or sets the content of a HTTP response message.
        //
        // Returns:
        //     Returns System.Net.Http.HttpContent.The content of the HTTP response message.
        object Content { get; set; }
        //
        // Summary:
        //     Gets the collection of HTTP response headers.
        //
        // Returns:
        //     Returns System.Net.Http.Headers.HttpResponseHeaders.The collection of HTTP
        //     response headers.
        HttpResponseHeaders Headers { get; set; }
        //
        // Summary:
        //     Gets a value that indicates if the HTTP response was successful.
        //
        // Returns:
        //     Returns System.Boolean.A value that indicates if the HTTP response was successful.
        //     true if System.Net.Http.HttpResponseMessage.StatusCode was in the range 200-299;
        //     otherwise false.
        bool IsSuccessStatusCode { get; set; }
        //
        // Summary:
        //     Gets or sets the reason phrase which typically is sent by servers together
        //     with the status code.
        //
        // Returns:
        //     Returns System.String.The reason phrase sent by the server.
        string ReasonPhrase { get; set; }
        //
        // Summary:
        //     Gets or sets the request message which led to this response message.
        //
        // Returns:
        //     Returns System.Net.Http.HttpRequestMessage.The request message which led
        //     to this response message.
        //HttpRequestMessage RequestMessage { get; set; }
        //
        // Summary:
        //     Gets or sets the status code of the HTTP response.
        //
        // Returns:
        //     Returns System.Net.HttpStatusCode.The status code of the HTTP response.
        HttpStatusCode StatusCode { get; set; }
        //
        // Summary:
        //     Gets or sets the HTTP message version.
        //
        // Returns:
        //     Returns System.Version.The HTTP message version. The default is 1.1.
        Version Version { get; set; }

    }

    //public interface IHttpObjectResult<T> : IHttpResult
    //{
    //}

    public class HttpObjectResult<T> : IHttpResult
    {

        public T Content
        {
            get
            {
                return (T)_content;
            }
            set
            {
                _content = value;
            }
        }

        private object _content;
        object IHttpResult.Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

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

        //public HttpRequestMessage RequestMessage
        //{
        //    get;
        //    set;
        //}

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
    }

    public class HttpStringResult : HttpObjectResult<string>
    {

    }
    public class HttpStreamResult : HttpObjectResult<Stream>
    {
    }

    public class HttpBinaryResult : HttpObjectResult<byte[]>
    {
    }


}
