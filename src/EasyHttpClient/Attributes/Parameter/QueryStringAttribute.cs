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
    public class QueryStringAttribute : Attribute, IParameterScopeAttribute
    {
        public QueryStringAttribute()
        {

        }
        public QueryStringAttribute(string name)
        {
            this.Name = name;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Query;
            }
        }
        public string Name
        {
            get;
            set;
        }

        internal void ProcessParameter(HttpRequestMessageBuilder requestBuilder, object parameterValue)
        {
            var pFormatAttr = requestBuilder.DefaultStringFormatter;
            requestBuilder.QueryStrings.AddRange(Utility.ExtractUrlParameter(this.Name, parameterValue, pFormatAttr, 1));
        }
        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var pFormatAttr = parameterInfo.GetCustomAttribute<StringFormatAttribute>() ?? requestBuilder.DefaultStringFormatter;
            requestBuilder.QueryStrings.AddRange(Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, pFormatAttr, 1));
        }
    }
}
