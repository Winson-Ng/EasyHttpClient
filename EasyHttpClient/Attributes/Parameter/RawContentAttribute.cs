using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RawContentAttribute : Attribute, IParameterAttribute
    {
        public RawContentAttribute() { 
        
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

        void IParameterAttribute.ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            if (parameterValue is Stream) {
                requestBuilder.StreamBody = Tuple.Create(this.ContentType, parameterValue as Stream);
            }
            else if (parameterValue is IEnumerable<byte>)
            {
                requestBuilder.StreamBody = Tuple.Create(this.ContentType, new MemoryStream((parameterValue as IEnumerable<byte>).ToArray()) as Stream);
            }
            else {
                var val = Convert.ToString(parameterInfo);
                requestBuilder.StreamBody = Tuple.Create(this.ContentType, new MemoryStream(Utf8Encoding.GetBytes(val)) as Stream);
            }
        }
    }
}
