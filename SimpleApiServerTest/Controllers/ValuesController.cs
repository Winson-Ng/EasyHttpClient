namespace SimpleApiServerTest.Controllers
{
    using SimpleApiServerTest.Code;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

    [RoutePrefix("api/value")]
    public class ValuesController : ApiController
    {
        TestModel _testModel;
        public ValuesController(TestModel testModel)
        {
            _testModel = testModel;
        }

        private static readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        [Route("{key}")]
        [Authorize]
        public async Task<IHttpActionResult> GetValue(string key)
        {
            object result;
            if (cache.TryGetValue(key, out result))
            {
                return Ok(result);
            }
            else {
                return NotFound();
            }
        }
        [Route("all")]
        [Authorize]
        public async Task<IHttpActionResult> GetValues()
        {
            return Ok(cache);
        }

        [Route("{key}")]
        [Authorize]
        [HttpPut,HttpPost]
        public async Task<IHttpActionResult> SetValue(string key, dynamic value)
        {
            cache[key] = value;
            return Ok();
        }

    }
}
