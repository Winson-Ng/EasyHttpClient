using EasyHttpClient.ActionFilters;
using EasyHttpClient.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class HttpClientSettings
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public IOAuth2ClientHandler OAuth2ClientHandler { get; set; }

        /// <summary>
        /// Define the global action filters.
        /// </summary>
        public IList<IActionFilter> ActionFilters { get; private set; }

        public int MaxRetry { get; set; }
        //private JsonSerializerSettings _jsonSerializerSettings;
        public JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                return this.JsonMediaTypeFormatter.SerializerSettings;
            }
            set
            {
                this.JsonMediaTypeFormatter.SerializerSettings = value;
                //this.JsonMediaTypeFormatter.SerializerSettings = value;
            }
        }

        public JsonMediaTypeFormatter JsonMediaTypeFormatter { get; private set; }

        /// <summary>
        /// To handle non json http request parameter datetime formatting, and it can be overrided by [StringFormatAttribute("format")]
        /// </summary>
        public String DateTimeFormat { get; set; }

        public HttpClientSettings()
        {
            this.JsonMediaTypeFormatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = DefaultJsonSerializerSettings
            };
            //this.JsonSerializerSettings = DefaultJsonSerializerSettings;
            this.ActionFilters = new List<IActionFilter>();
        }
    }
}
