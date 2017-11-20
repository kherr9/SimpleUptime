using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SimpleUptime.Domain.Models;
using SimpleUptime.IntegrationTests.Fixtures;
using SimpleUptime.IntegrationTests.WebApi.Controllers.Client;
using SimpleUptime.IntegrationTests.WebApi.Controllers.Helpers;
using Xunit;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers
{
    public class HttpMonitorTestControllerTests : IClassFixture<WebApiAppFixture>, IDisposable
    {
        private readonly WebApiAppFixture _webApiAppFixture;
        private readonly HttpMonitorClient _client;
        private readonly IWebHost _testServer;

        public HttpMonitorTestControllerTests(WebApiAppFixture webApiAppFixture)
        {
            _webApiAppFixture = webApiAppFixture;
            _client = new HttpMonitorClient(webApiAppFixture.HttpClient);

            _testServer = DummyHttpTestServer.CreateAndRunWebHost("http://localhost:5000");
        }

        public void Dispose()
        {
            _webApiAppFixture.Reset();
            _testServer.Dispose();
        }

        [Fact]
        public async Task NotFound()
        {
            // Arrange
            var id = HttpMonitorId.Create();

            // Act
            (var response, _) = await _client.TestAsync(id.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpMonitorIdHelper.InvalidHttpMonitorIds), MemberType = typeof(HttpMonitorIdHelper))]
        public async Task InvalidHttpMonitorId(object id)
        {
            // Act
            (var response, _) = await _client.TestAsync(id.ToString());

            // Arrange
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test()
        {
            // Arrange
            (_, var entity) = await _client.PostAsync(new
            {
                Url = "http://localhost:5000/"// _testServer.BaseAddress
            });

            string actualMethod = null;
            string actualPath = null;
            string actualQueryString = null;
            DummyHttpTestServer.Handler = ctx =>
            {
                actualMethod = ctx.Request.Method;
                actualPath = ctx.Request.Path.Value;
                actualQueryString = ctx.Request.QueryString.Value;

                return Task.CompletedTask;
            };

            // Act
            (var response, var thing) = await _client.TestAsync(entity.Id);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpMonitorDto> GenerateAndPostHttpMonitorAsync()
        {
            var entity = EntityGenerator.GenerateHttpMonitor();

            return (await _client.PostAsync(entity)).Item2;
        }
    }
}
