using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.Services;
using Xunit;

using HttpRequest = SimpleUptime.Domain.Models.HttpRequest;
using HttpResponse = SimpleUptime.Domain.Models.HttpResponse;

namespace SimpleUptime.IntegrationTests.Infrastructure.Services
{
    public class HttpMonitorExecutorTests : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpMonitorExecutor _executor;

        public HttpMonitorExecutorTests()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .UseStartup<AnonymousStartup>());

            _executor = new HttpMonitorExecutor(_testServer.CreateClient());
        }

        [Theory]
        [InlineData("Delete")]
        [InlineData("Get")]
        [InlineData("Head")]
        [InlineData("Options")]
        [InlineData("Post")]
        [InlineData("Put")]
        [InlineData("Trace")]
        public async Task RequestHttpMethodAndPathCalled(string httpMethod)
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = new Uri(_testServer.BaseAddress, $"/api/{DateTime.UtcNow.Ticks}/index.html?q={DateTime.UtcNow.Ticks}"),
                    Method = new HttpMethod(httpMethod)
                }
            };
            
            string actualHttpMethod = null;
            string actualRelativePath = null;
            string actualQueryString = null;
            AnonymousStartup.RequestDelegate = ctx =>
            {
                // capture incoming http method
                actualHttpMethod = ctx.Request.Method;
                actualRelativePath = ctx.Request.Path.Value;
                actualQueryString = ctx.Request.QueryString.Value;

                return Task.CompletedTask;
            };

            // Act
            var @event = await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Equal(command.Request.Method.Method, actualHttpMethod, StringComparer.InvariantCultureIgnoreCase);
            Assert.Equal(command.Request.Url.AbsolutePath, actualRelativePath);
            Assert.Equal(command.Request.Url.Query, actualQueryString);
        }

        public class AnonymousStartup
        {
            public static RequestDelegate RequestDelegate;

            public void Configure(IApplicationBuilder applicationBuilder)
            {
                applicationBuilder.Run(ctx =>
                {
                    return RequestDelegate?.Invoke(ctx) ?? Task.CompletedTask;
                });
            }
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }
}
