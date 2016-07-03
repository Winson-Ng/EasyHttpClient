using EasyHttpClient;
using EasyHttpClientTest.ApiClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyHttpClientTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = "http://localhost:60944";
            var factory = new HttpClientWrapperFactory()
            {
                DefaultHost = new Uri(host)
            };
            factory.HttpClientSettings.MaxRetry = 3;

            var oauthApiClient = new HttpClientWrapperFactory().CreateFor<OAuthApiClient>(host);
            //var oauth2ClientHandler = new OAuth2ClientHandler(oauthApiClient);
            //factory.HttpClientSettings.OAuth2ClientHandler = oauth2ClientHandler;

            //oauth2ClientHandler.Login();

            var testApiClient = factory.CreateFor<TestApiClient>();
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                //testApiClient.SetValue("myname", line);
                var val = testApiClient.GetValue("myname");

                Console.WriteLine("val=" + val);
            }
        }
    }
}
