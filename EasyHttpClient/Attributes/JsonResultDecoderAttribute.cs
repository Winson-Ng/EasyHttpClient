using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class JsonResultDecoderAttribute : HttpResultDecoderAttribute
    {
        public override bool CanDecode(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            return true;
        }

        public override async Task<object> DecodeAsync(System.Net.Http.HttpContent httpContent, ActionContext actionContext)
        {
            return await httpContent.ReadAsAsync(actionContext.ReturnTypeDescription.TargetObjectType, new[] { actionContext.HttpClientSettings.JsonMediaTypeFormatter });            
        }
    }
}
