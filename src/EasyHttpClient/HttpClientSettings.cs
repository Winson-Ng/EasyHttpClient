using EasyHttpClient.ActionFilters;
using EasyHttpClient.Attributes;
using EasyHttpClient.OAuth2;
using EasyHttpClient.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class HttpClientSettings
    {
        private static readonly Func<JsonSerializerSettings> DefaultJsonSerializerSettings = () => new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>() { new JsonIntegerConverter() }
        };
        public IOAuth2ClientHandler OAuth2ClientHandler { get; set; }

        public IList<Func<DelegatingHandler>> DelegatingHandlers { get; private set; }
        /// <summary>
        /// Define the global action filters.
        /// </summary>
        public IList<IActionFilter> ActionFilters { get; private set; }

        public bool AutomaticDecompression { get; set; }

        /// <summary>
        /// The default value is 100,000 milliseconds (100 seconds).
        /// To set an infinite timeout, set the property value to System.Threading.Timeout.InfiniteTimeSpan (-00:00:00.0010000, or -1 millisecond.)
        /// default infinite timeout.
        /// A Domain Name System (DNS) query may take up to 15 seconds to return or time out. If your request contains a host name that requires resolution and you set Timeout to a value less than 15 seconds, it may take 15 seconds or more before a WebException is thrown to indicate a timeout on your request.
        /// The same timeout will apply for all requests using this HttpClient instance. You may also set different timeouts for individual requests using a CancellationTokenSource on a task. Note that only the shorter of the two timeouts will apply.
        /// </summary>
        public TimeSpan Timeout { get; set; }
        public int MaxRetry { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
        public JsonMediaTypeFormatter JsonMediaTypeFormatter { get; private set; }
        public IHttpResultDecoder HttpResultDecoder { get; set; }
        /// <summary>
        /// To handle non json http request parameter datetime formatting, and it can be overrided by [StringFormatAttribute("format")]
        /// </summary>
        public String DateTimeFormat { get; set; }

        public HttpClientSettings()
        {
            this.AutomaticDecompression = true;
            this.JsonSerializerSettings = DefaultJsonSerializerSettings();
            this.JsonMediaTypeFormatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = this.JsonSerializerSettings
            };
            this.ActionFilters = new List<IActionFilter>();
            this.DelegatingHandlers = new List<Func<DelegatingHandler>>();
            this.Timeout = TimeSpan.FromMilliseconds(100000);
        }
    }
}
