using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizeAttribute : Attribute
    {
    }
}
