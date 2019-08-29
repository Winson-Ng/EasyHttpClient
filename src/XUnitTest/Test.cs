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
            var factory = new EasyHttpClientFactory();
            var baiduApiClient = factory.Create<IBaiduApiClient>("https://m.baidu.com/");
            var response = await baiduApiClient.SearchSugguestion("Are You OK");
            Assert.Equal(200, (int)response.StatusCode);
        }
    }
}
