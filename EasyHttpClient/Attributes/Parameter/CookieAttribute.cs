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
    public class CookieAttribute : Attribute, IParameterScopeAttribute
    {
        public CookieAttribute()
        {

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

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var pFormatAttr = parameterInfo.GetCustomAttribute<StringFormatAttribute>() ?? requestBuilder.DefaultStringFormatter;
            var pathParams = Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, pFormatAttr, 1);
            foreach (var p in pathParams)
            {
                requestBuilder.Cookies.Add(new CookieHeaderValue(p.Key, p.Value));
            }
        }
    }
}
