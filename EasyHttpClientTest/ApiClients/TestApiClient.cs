using EasyHttpClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClientTest.ApiClients
{
    [RoutePrefix("api/value")]
    public interface TestApiClient
    {
        [Route("{key}")]
        [Authorize]
        [HttpGet]
        object GetValue(string key);

        [Route("{key}")]
        [Authorize]
        [HttpPut]
        void SetValue(string key, dynamic value);
    }
}
