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
    public class QueryStringAttribute : Attribute, IParameterAttribute
    {
        public QueryStringAttribute() { 
        
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
        
        void IParameterAttribute.ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            requestBuilder.QueryStrings.AddRange(Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, 1));
        }
    }
}
