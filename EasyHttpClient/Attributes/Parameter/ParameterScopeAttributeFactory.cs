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
    internal class ParameterScopeAttributeFactory
    {
        public static IParameterScopeAttribute CreateByHttpMethod(HttpMethod httpMethod, ParameterInfo parameterInfo)
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

}
