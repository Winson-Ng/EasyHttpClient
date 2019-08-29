using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    public class MultiPartType
    {
        private MultiPartType() { 
        
        }
        public const string Mixed = "mixed";
        public const string FormData = "form-data";
        public const string Related = "Related";
    }
    /// <summary>
    /// MultiPartAttribute will parse the http request body as multiple part.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MultiPartAttribute : Attribute
    {
        public string MultiPartType { get; set; }
        public MultiPartAttribute(string multiPartType)
        {
            this.MultiPartType = multiPartType;
        }

        public MultiPartAttribute()
            : this(EasyHttpClient.Attributes.MultiPartType.FormData)
        {
        }
    }
}
