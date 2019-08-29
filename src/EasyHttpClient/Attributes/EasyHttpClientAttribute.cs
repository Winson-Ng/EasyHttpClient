using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EasyHttpClient.Attributes
{
    public class EasyHttpClientAttribute
    {
        public string Name { get; set; }
        public Uri Host { get; set; }
        public HttpClient HttpClient { get; set; }
        public HttpClientSettings HttpClientSettings { get; set; } = new HttpClientSettings();
        public bool AuthorizeRequired { get; set; }
        public EasyHttpClientAttribute()
        {
            new HttpClientSettings();
        }
    }
}
