using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class OpenHttpServerTests : IDisposable
    {
        private readonly OpenHttpServer _httpServer;
        private readonly HttpClient _client;

        public OpenHttpServerTests()
        {
            _httpServer = OpenHttpServer.CreateAndRun();
            _client = _httpServer.CreateClient();
        }

        public void Dispose()
        {
            _httpServer?.Dispose();
            _client?.Dispose();
        }

        [Fact]
        public async Task SetConentAndStatusCode()
        {
            // Arrange
            _httpServer.Handler = async ctx =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Accepted;
                await ctx.Response.WriteAsync("Hello World", Encoding.UTF8);
            };

            // Act
            var response = await _client.GetAsync("");

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.Equal("Hello World", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task NotSettingHandlerReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}