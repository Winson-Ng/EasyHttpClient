using EasyHttpClient;
using EasyHttpClient.Attributes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTest
{
    [RoutePrefix("")]
    public interface IBaiduApiClient
    {
        [Route("sugrec")]
        [HttpGet]
        Task<IHttpResult<string>> SearchSugguestion(string wd, string prod= "wise");
    }
}
