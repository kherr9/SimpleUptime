using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace SimpleUptime.IntegrationTests.Infrastructure.Services
{
    public class HttpMonitorExecutorTests: IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _httpClient;

        public HttpMonitorExecutorTests()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .UseStartup<AnonymousStartup>());
        }


        [Fact]
        public async Task Get200()
        {
            // Arrange
            RequestDelegate handle = ctx =>
            {
                if (ctx.Request.Path)
            };

            Action<IApplicationBuilder> configure = app => app.Map("index.html", map =>
            {
                map.Run(ctx =>
                {
                    ctx.Response.StatusCode = 200;

                    return Task.CompletedTask;
                });
            });
        }

        private void StartServer(RequestDelegate requestDelegate)
        {
            var x = new AnonymousStartup(requestDelegate);
            var builder = new WebHostBuilder().UseStartup<AnonymousStartup>();
            _testServer = new TestServer(x);

        }

        public class AnonymousStartup
        {
            private readonly RequestDelegate _requestDelegate;

            public AnonymousStartup(RequestDelegate requestDelegate)
            {
                _requestDelegate = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
            }

            public void Configuration(IApplicationBuilder applicationBuilder)
            {
                applicationBuilder.Map("*", map => map.Run(_requestDelegate));
            }
        }

        public void Dispose()
        {
            _testServer?.Dispose();
            _httpClient?.Dispose();
        }
    }
}
