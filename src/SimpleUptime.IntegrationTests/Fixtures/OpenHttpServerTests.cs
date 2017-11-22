using System;
using System.Collections.Generic;
using System.Linq;
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

        [Theory]
        [MemberData(nameof(HttpStatusCodes))]
        public async Task HttpStatusCodes(HttpStatusCode statusCode)
        {
            // Arrange
            _httpServer.Handler = ctx =>
            {
                ctx.Response.StatusCode = (int)statusCode;
                return Task.CompletedTask;
            };

            // Act
            var response = await _client.GetAsync(_httpServer.BaseAddress);

            // Assert
            Assert.Equal(statusCode, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpRequestMethods))]
        public async Task HttpRequestMethods(string method)
        {
            // Arrange
            _httpServer.Handler = ctx => Task.CompletedTask;

            var request = new HttpRequestMessage(new HttpMethod(method), _httpServer.BaseAddress.ToString());

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public static IEnumerable<object[]> HttpRequestMethods()
        {
            //yield return new object[] { HttpMethods.Connect };
            yield return new object[] { HttpMethods.Put };
            yield return new object[] { HttpMethods.Post };
            yield return new object[] { HttpMethods.Patch };
            yield return new object[] { HttpMethods.Trace };
            yield return new object[] { HttpMethods.Head };
            yield return new object[] { HttpMethods.Get };
            yield return new object[] { HttpMethods.Delete };
            yield return new object[] { HttpMethods.Options };
        }

        public static IEnumerable<object[]> HttpStatusCodes()
        {
            foreach (var value in Enum.GetValues(typeof(HttpStatusCode)).Cast<HttpStatusCode>().Where(x => (int)x >= 200))
            {
                yield return new object[] { value };
            }
        }
    }
}