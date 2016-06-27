using EasyHttpClient.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FormBodyAttribute : Attribute, IParameterAttribute
    {
        public FormBodyAttribute() { 
        
        }
        public FormBodyAttribute(string name)
        {
            this.Name = name;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Form;
            }
        }

        public string Name
        {
            get;
            set;
        }

        void IParameterAttribute.ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            requestBuilder.FormBodys.AddRange(Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, 1));
        }
    }
}
