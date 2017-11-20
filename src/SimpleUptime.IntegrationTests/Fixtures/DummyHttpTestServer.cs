using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class DummyHttpTestServer
    {
        public static RequestDelegate Handler;

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Run(ctx => Handler?.Invoke(ctx) ?? Task.CompletedTask);
        }

        public static TestServer CreateTestServer()
        {
            return new TestServer(new WebHostBuilder()
                .UseStartup<DummyHttpTestServer>()
                .UseUrls("http://localhost:5051/"));
        }

        public static IWebHost CreateAndRunWebHost(string url)
        {
            var webhost = WebHost.CreateDefaultBuilder(new string[0])
                ////.UseHttpSys(options =>
                ////{
                ////    ////options.Authentication.Schemes = AuthenticationSchemes.Negotiate | AuthenticationSchemes.NTLM;
                ////    ////options.Authentication.AllowAnonymous = false;
                ////    options.UrlPrefixes.Add("http://localhost:5000");
                ////})
                .UseStartup<DummyHttpTestServer>()
                .UseUrls("http://localhost:5000")
                .Build();

            webhost.RunAsync();

            return webhost;
        }
    }
}
