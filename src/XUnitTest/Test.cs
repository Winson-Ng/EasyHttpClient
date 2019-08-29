using EasyHttpClient;
using System;
using Xunit;

namespace XUnitTest
{
    public class Test
    {
        [Fact]
        public async void SendTest()
        {
            var factory = new HttpClientWrapperFactory();
            factory.Host = new Uri("https://m.baidu.com/");
            var baiduApiClient = factory.CreateFor<IBaiduApiClient>();
            var result = await baiduApiClient.SearchSugguestion("Are You OK");
            Assert.Equal(200, (int)result.StatusCode);
        }
    }
}
