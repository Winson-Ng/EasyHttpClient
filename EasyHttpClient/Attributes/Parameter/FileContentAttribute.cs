using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FileContentAttribute : Attribute, IParameterScopeAttribute
    {
        public FileContentAttribute()
        {

        }

        public FileContentAttribute(string name)
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

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var dispositionName = "\""+(!string.IsNullOrWhiteSpace(this.Name) ? this.Name : parameterInfo.Name)+"\"";
            var contentDisposition = new ContentDispositionHeaderValue(
                    requestBuilder.MultiPartAttribute != null ? requestBuilder.MultiPartAttribute.MultiPartType : MultiPartType.FormData
                    );

            if (parameterValue is string)
            {
                var f = new FileInfo(parameterValue.ToString());
                var content = new StreamContent(f.OpenRead());
                content.Headers.ContentDisposition = contentDisposition;
                content.Headers.ContentDisposition.FileName = f.Name;
                content.Headers.ContentDisposition.Name = dispositionName;
                content.Headers.ContentType = new MediaTypeHeaderValue(!string.IsNullOrWhiteSpace(this.ContentType) ? this.ContentType : MimeMapping.GetMimeMapping(f.Name));

                requestBuilder.RawContents.Add(content);
            }
            else if (parameterValue is FileInfo)
            {
                var f = parameterValue as FileInfo;
                var content = new StreamContent(f.OpenRead());
                content.Headers.ContentDisposition = contentDisposition;
                content.Headers.ContentDisposition.FileName = f.Name;
                content.Headers.ContentDisposition.Name = dispositionName;
                content.Headers.ContentType = new MediaTypeHeaderValue(!string.IsNullOrWhiteSpace(this.ContentType) ? this.ContentType : MimeMapping.GetMimeMapping(f.Name));

                requestBuilder.RawContents.Add(content);
            }
            else if (parameterValue is FileStream)
            {
                var s = parameterValue as FileStream;
                var content = new StreamContent(s);
                content.Headers.ContentDisposition = contentDisposition;
                content.Headers.ContentDisposition.FileName = s.Name;
                content.Headers.ContentDisposition.Name = dispositionName;
                content.Headers.ContentType = new MediaTypeHeaderValue(!string.IsNullOrWhiteSpace(this.ContentType) ? this.ContentType : MimeMapping.GetMimeMapping(s.Name));
                requestBuilder.RawContents.Add(content);
            }
            else
            {
                throw new NotSupportedException("parameterValue must be String/FileInfo/FileStream");
            }
        }
    }
}
