using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class PathParamAttribute : Attribute, IParameterAttribute
    {
        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Path;
            }
        }
        public string Name
        {
            get;
            set;
        }

        void IParameterAttribute.ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            requestBuilder.PathParams.AddRange(Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, 1));
        }
    }
}
