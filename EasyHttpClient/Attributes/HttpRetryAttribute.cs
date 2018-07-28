using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    /// <summary>
    /// Auto retry the http request when HttpStatus = 408 OR HttpStatus>500
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false)]
    public class HttpRetryAttribute : Attribute
    {
        /// <summary>
        /// 0=Disable, default use HttpClientSettings.MaxRetry
        /// </summary>
        public int MaxRetry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxRetry">
        /// 0=Disable, default use HttpClientSettings.MaxRetry
        /// </param>
        public HttpRetryAttribute(int maxRetry)
        {
            this.MaxRetry = maxRetry;
        }

        public HttpRetryAttribute()
        {

        }
    }
}
