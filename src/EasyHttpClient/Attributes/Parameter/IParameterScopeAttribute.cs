using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    public interface IParameterScopeAttribute
    {
        string Name { get; }
        ParameterScope Scope { get; }

        void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue);

    }
}
