using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class HeaderAttribute : Attribute, IParameterScopeAttribute
    {
        public HeaderAttribute() { 
        
        }
        public HeaderAttribute(string name) {
            this.Name = name;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Header;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var pFormatAttr = parameterInfo.GetCustomAttribute<StringFormatAttribute>() ?? requestBuilder.DefaultStringFormatter;
            requestBuilder.Headers.AddRange(Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, pFormatAttr ,1));
        }
    }
}
