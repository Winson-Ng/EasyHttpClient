using EasyHttpClient.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    public enum ParameterScope
    {
        Query = 0,
        Path = 2,
        Header = 3,
        Cookie = 4,
        Form = 5,
        Json = 6,
        RawContent = 7
    }

    internal class ParameterAttributeFactory
    {
        //public string Name { get; set; }
        //public ParameterScope Scope { get; set; }
        //public object DefaultValue { get; set; }

        public static IParameterAttribute CreateByHttpMethod(HttpMethod httpMethod, ParameterInfo parameterInfo)
        {
            if (typeof(Stream).IsAssignableFrom(parameterInfo.ParameterType) || typeof(IEnumerable<byte>).IsAssignableFrom(parameterInfo.ParameterType))
            {
                return new RawContentAttribute() { Name = parameterInfo.Name, ContentType = "application/o-stream" };
            }
            else if (httpMethod == HttpMethod.Post)
            {
                return new FormBodyAttribute() { Name = parameterInfo.Name };
            }
            else if (httpMethod == HttpMethod.Put)
            {
                return new JsonBodyAttribute() { Name = parameterInfo.Name };
            }
            else
            {
                return new QueryStringAttribute() { Name = parameterInfo.Name };
            }
        }

    }

    internal interface IParameterAttribute
    {
        string Name { get; }
        ParameterScope Scope { get; }

        void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue);

    }
}
