using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CookieAttribute : Attribute, IParameterAttribute
    {
        public CookieAttribute() { 
        
        }
        public CookieAttribute(string name)
        {
            this.Name = name;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Cookie;
            }
        }

        public string Name
        {
            get;
            set;
        }

        void IParameterAttribute.ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var pathParams = Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, 1);
            foreach (var p in pathParams)
            {
                requestBuilder.Cookies.Add(new CookieHeaderValue(p.Key, p.Value));
            }
        }
    }
}
