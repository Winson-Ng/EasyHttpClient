using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes.Parameter
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class HttpAliasAttribute : Attribute
    {
        public String Name { get; private set; }
        public HttpAliasAttribute(string name)
        {
            this.Name = name;
        }
    }
}
