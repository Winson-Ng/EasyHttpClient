
using SimpleApiServerTest.Code;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Threading;


namespace SimpleApiServerTest.Controllers
{

    public class TestRequestModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public MultipartFileData File { get; set; }
    }

    [RoutePrefix("api/value")]
    public class ValuesController : ApiController
    {

        TestModel _testModel;
        public ValuesController(TestModel testModel)
        {
            _testModel = testModel;
        }

        private static readonly Dictionary<string, object> cache = new Dictionary<string, object>() { 
        {"hello","hello"}
        };

        [Route("{key}")]
        //[Authorize]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetValue(string key)
        {
            await Task.Delay(60000);
            object result;
            if (cache.TryGetValue(key, out result))
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
        [Route("all")]
        [Authorize]
        public Task<IHttpActionResult> GetValues()
        {

            return Task.Delay(30000).ContinueWith<IHttpActionResult>((t) => Ok(cache));
        }

        [Route("{key}")]
        [Authorize]
        [HttpPut, HttpPost]
        public async Task<IHttpActionResult> SetValue(string key, dynamic value)
        {
            cache[key] = value;
            return Ok();
        }

        [Route("upload")]
        [HttpPost]
        public Task<IHttpActionResult> testRequestModel(TestRequestModel testRequestModel)
        {
            return Task.Delay(30000).ContinueWith<IHttpActionResult>((t) => Ok());
        }
    }
}
