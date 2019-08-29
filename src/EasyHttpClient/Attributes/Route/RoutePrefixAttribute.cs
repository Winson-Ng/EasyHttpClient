using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false)]
    public class RoutePrefixAttribute : Attribute
    {
        // Summary:
        //     Initializes a new instance of the System.Web.Http.RouteAttribute class.
        public RoutePrefixAttribute()
        {

        }
        // Summary:
        //     Initializes a new instance of the System.Web.Http.RouteAttribute class.
        //
        // Parameters:
        //   template:
        //     The route template describing the URI pattern to match against.
        public RoutePrefixAttribute(string prefix)
        {
            this.Prefix = prefix;
        }
        //
        //
        // Returns:
        //     Returns System.String.
        public string Prefix { get; set; }

    }
}
