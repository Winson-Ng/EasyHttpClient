# EasyHttpClient

### What's EasyHttpClient?

EasyHttpClient provide you an easy way to access HTTP resource(e.g. REST API client, html, stream, upload and down).

## Usage
### 3 steps to go

#### Step 1, create your http client interface

...

    [RoutePrefix("api/value")]
    public interface TestApiClient
    {
        [Route("{key}")]
        [Authorize]
        [HttpGet]
        object GetValue(string key, string def="test");

        [Route("{key}")]
        [HttpPut]
        void SetValue([PathParam][JsonBody]string key, [JsonBody]dynamic value);
    }
    
...

#### Step 2, config the HttpClientWrapperFactory

...

    var factory = new HttpClientWrapperFactory()
    {
        DefaultHost = new Uri(host)
    };
    
...

#### Step 3, use in my code

...

    var testApiClient = factory.CreateFor<TestApiClient>();
    var val = testApiClient.GetValue("myname");
  
...

### Config OAuth2
...

    factory.HttpClientSettings.OAuth2ClientHandler = new MyOAuth2ClientHandler();
    
...


### Extend HttpClientProvider, to add my handlers on System.Net.Http.HttpClient class

...

    factory.HttpClientProvider = new HttpClientProvider();
    
...

### Extend EasyHttpClient.ActionFilters.ActionFilterAttribute class to enhance your function, for example logging, cache, validation

...

    [RoutePrefix("api/value")]
    public interface TestApiClient
    {
        [Route("{key}")]
        [Authorize]
        [HttpGet]
        [ApiClientCache]
        object GetValue(string key, string def="test");

        [Route("{key}")]
        [HttpPut]
        void SetValue([PathParam][JsonBody]string key, [JsonBody]dynamic value);
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiClientCacheAttribute : ActionFilterAttribute
    {
        public static ICacheClient CacheClient { get; set; }

        public override Task<IHttpResult> ActionInvoke(ActionContext context, Func<Task<IHttpResult>> Continuation)
        {
            //TODO: handle cache
            return Continuation();
        }
    }
    
...





