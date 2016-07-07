using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyHttpClient.Utilities
{
    public class HttpRequestMessageBuilder
    {
        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);

        public HttpMethod HttpMethod { get; set; }
        public UriBuilder UriBuilder { get; set; }
        public JsonSerializer JsonSerializer { get; private set; }
        private JsonSerializerSettings _jsonSetting;
        public JsonSerializerSettings JsonSetting
        {
            get
            {
                return _jsonSetting;
            }
            set
            {
                _jsonSetting = value;
                this.JsonSerializer = JsonSerializer.Create(value);
            }
        }

        public List<KeyValuePair<string, string>> Headers { get; private set; }
        public List<CookieHeaderValue> Cookies { get; private set; }
        public List<KeyValuePair<string, string>> PathParams { get; private set; }
        public List<KeyValuePair<string, string>> QueryStrings { get; private set; }
        public List<KeyValuePair<string, string>> FormBodys { get; private set; }
        public JToken JsonBody { get; set; }
        public Tuple<string, Stream> StreamBody { get; set; }

        public HttpRequestMessageBuilder(HttpMethod httpMethod, UriBuilder uriBuilder, JsonSerializerSettings jsonSetting)
        {
            this.HttpMethod = httpMethod;
            this.UriBuilder = uriBuilder;
            this.JsonSetting = jsonSetting;

            this.Headers = new List<KeyValuePair<string, string>>();
            this.Cookies = new List<CookieHeaderValue>();
            this.PathParams = new List<KeyValuePair<string, string>>();
            this.QueryStrings = new List<KeyValuePair<string, string>>();
            this.FormBodys = new List<KeyValuePair<string, string>>();

        }

        public HttpRequestMessage Build()
        {
            var httpMessage = new HttpRequestMessage(HttpMethod, UriBuilder.Uri);


            foreach (var h in this.Headers.GroupBy(h => h.Key))
                httpMessage.Headers.TryAddWithoutValidation(h.Key, h.Select(i => i.Value));

            foreach (var c in this.Cookies)
                httpMessage.Headers.GetCookies().Add(c);

            if (this.PathParams.Any())
            {
                var path = HttpUtility.UrlDecode(UriBuilder.Path);
                foreach (var p in this.PathParams.GroupBy(p => p.Key))
                {
                    path = path.Replace("{" + p.Key + "}", string.Join(",", p.Select(i => i.Value)));
                }
                UriBuilder.Path = HttpUtility.UrlPathEncode(path);
            }

            if (this.QueryStrings.Any())
                UriBuilder.Query = string.Join("&", this.QueryStrings.Select(q => q.Key + "=" + HttpUtility.UrlEncode(q.Value)));

            if (this.HttpMethod == HttpMethod.Post
                || this.HttpMethod == HttpMethod.Put)
            {
                if (new[]{
                FormBodys.Any(),
                JsonBody!=null,
                StreamBody!=null
            }.Count(i => i == true) > 1)
                {
                    throw new NotSupportedException("Not support multiple kinds of http content in a message!");
                }

                if (FormBodys.Any())
                {
                    httpMessage.Content = new FormUrlEncodedContent(this.FormBodys);
                }
                else if (JsonBody != null)
                {
                    httpMessage.Content = new StringContent(JsonBody.ToString(), Utf8Encoding, "application/json");
                }
                else if (StreamBody != null)
                {
                    httpMessage.Content = new StreamContent(this.StreamBody.Item2);
                    httpMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(StreamBody.Item1);
                }
            }
            httpMessage.RequestUri = this.UriBuilder.Uri;

            return httpMessage;
        }
    }
}
