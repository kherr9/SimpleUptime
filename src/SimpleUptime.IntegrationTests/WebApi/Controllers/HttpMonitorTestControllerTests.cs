using System;
using System.Net;
using System.Threading.Tasks;
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

        public HttpMonitorTestControllerTests(WebApiAppFixture webApiAppFixture)
        {
            _webApiAppFixture = webApiAppFixture;
            _client = new HttpMonitorClient(webApiAppFixture.HttpClient);
        }

        public void Dispose()
        {
            _webApiAppFixture.Reset();
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

        private async Task<HttpMonitorDto> GenerateAndPostHttpMonitorAsync()
        {
            var entity = EntityGenerator.GenerateHttpMonitor();

            return (await _client.PostAsync(entity)).Item2;
        }
    }
}
