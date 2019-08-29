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
    public class FormBodyAttribute : Attribute, IParameterScopeAttribute
    {
        public FormBodyAttribute()
        {

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


        internal void ProcessParameter(HttpRequestMessageBuilder requestBuilder,object parameterValue)
        {
            var pFormatAttr = requestBuilder.DefaultStringFormatter;

            if (requestBuilder.MultiPartAttribute != null)
            {
                var multiPartType = requestBuilder.MultiPartAttribute.MultiPartType;
                if (string.IsNullOrWhiteSpace(multiPartType))
                    multiPartType = MultiPartType.FormData;
                requestBuilder.RawContents.AddRange(Utility.ExtractMultipleFormContent(this.Name, parameterValue, pFormatAttr, 1, multiPartType));
            }
            else
            {
                requestBuilder.FormBodys.AddRange(Utility.ExtractUrlParameter(this.Name, parameterValue, pFormatAttr, 1));
            }
        }

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {
            var pFormatAttr = parameterInfo.GetCustomAttribute<StringFormatAttribute>() ?? requestBuilder.DefaultStringFormatter;

            if (requestBuilder.MultiPartAttribute != null)
            {
                var multiPartType = requestBuilder.MultiPartAttribute.MultiPartType;
                if (string.IsNullOrWhiteSpace(multiPartType))
                    multiPartType = MultiPartType.FormData;
                requestBuilder.RawContents.AddRange(Utility.ExtractMultipleFormContent(this.Name ?? parameterInfo.Name, parameterValue, pFormatAttr, 1, multiPartType));
            }
            else
            {
                requestBuilder.FormBodys.AddRange(Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, pFormatAttr, 1));
            }

        }
    }
}
