using EasyHttpClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{

    internal class ParameterWithAttributes
    {
        public ParameterInfo ParameterInfo { get; set; }
        public IParameterAttribute[] ParameterAttributes { get; set; }
    }

    internal class RequestMethodParameters
    {
        public string Route { get; set; }
        public bool AuthorizeRequired { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public ParameterWithAttributes[] ParameterInfos { get; set; }
    }
}
