using EasyHttpClient.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    /// <summary>
    /// Support type:
    /// 1. HttpContent
    /// 2. Stream
    /// 3. byte[]
    /// 4. String
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RawContentAttribute : Attribute, IParameterScopeAttribute
    {
        public RawContentAttribute()
        {

        }
        public RawContentAttribute(string name)
        {
            this.Name = name;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.RawContent;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string ContentType { get; set; }

        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            Action<object> process = (pv) =>
            {
                HttpContent content = null;
                if (pv is HttpContent)
                {
                    content = pv as HttpContent;
                }
                else
                {

                    var dispositionName = "\"" + (!string.IsNullOrWhiteSpace(this.Name) ? this.Name : parameterInfo.Name) + "\"";
                    var contentDisposition = new ContentDispositionHeaderValue(
                            requestBuilder.MultiPartAttribute != null ? requestBuilder.MultiPartAttribute.MultiPartType : MultiPartType.FormData
                            );

                    if (pv is Stream)
                    {
                        var s = pv as Stream;
                        content = new StreamContent(s);
                    }
                    else if (pv is IEnumerable<byte>)
                    {
                        var s = new MemoryStream((pv as IEnumerable<byte>).ToArray());
                        content = new StreamContent(s);
                    }
                    else
                    {
                        var val = Convert.ToString(pv);
                        content = new StringContent(val);
                    }

                    if (!string.IsNullOrWhiteSpace(this.ContentType))
                        content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);

                    content.Headers.ContentDisposition = contentDisposition;
                    content.Headers.ContentDisposition.Name = dispositionName;

                }

                requestBuilder.RawContents.Add(content);
            };

            if (parameterValue != null)
            {
                if (typeof(IEnumerable).IsAssignableFrom(parameterValue.GetType()))
                {
                    foreach (var pv in parameterValue as IEnumerable)
                    {
                        process(pv);
                    }
                }
                else if (parameterValue.GetType().IsArray)
                {
                    foreach (var pv in parameterValue as Array)
                    {
                        process(pv);
                    }
                }
                else
                {
                    process(parameterValue);
                }
            }
            else
            {
                throw new NotSupportedException("parameterValue not allow null");
            }
        }
    }
}
