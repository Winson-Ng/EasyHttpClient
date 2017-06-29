using EasyHttpClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class ReturnTypeDescription
      {
        internal ReturnTypeDescription()
        {
            this.HttpResultType = typeof(HttpResult<>);
        }
        public Type ReturnType { get; internal set; }
        public Type TargetObjectType { get; internal set; }
        public IHttpResultDecoder HttpResultDecoder { get; internal set; }

        public Type HttpResultType { get; internal set; }
      }
}
