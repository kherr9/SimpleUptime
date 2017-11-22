using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.Services;
using SimpleUptime.IntegrationTests.Fixtures;
using Xunit;

using HttpRequest = SimpleUptime.Domain.Models.HttpRequest;
using HttpResponse = SimpleUptime.Domain.Models.HttpResponse;

namespace SimpleUptime.IntegrationTests.Infrastructure.Services
{
    public class HttpMonitorExecutorTests : IDisposable
    {
        private readonly HttpMonitorExecutor _executor;
        private readonly HttpClient _httpClient;
        private readonly OpenHttpServer _httpServer;

        public HttpMonitorExecutorTests()
        {
            _httpServer = OpenHttpServer.CreateAndRun();

            // todo configure client with custom logic since it needs to be configured per rules of executor
            _httpClient = _httpServer.CreateClient();
            _executor = new HttpMonitorExecutor(_httpClient);
        }

        public void Dispose()
        {
            _httpServer?.Dispose();
        }

        [Theory]
        [MemberData(nameof(OpenHttpServerTests.HttpRequestMethods), MemberType = typeof(OpenHttpServerTests))]
        public async Task RequestHttpMethodAndPathCalled(string httpMethod)
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = new Uri(_httpServer.BaseAddress, $"/api/{DateTime.UtcNow.Ticks}/index.html?q={DateTime.UtcNow.Ticks}"),
                    Method = new HttpMethod(httpMethod)
                }
            };

            string actualHttpMethod = null;
            string actualRelativePath = null;
            string actualQueryString = null;
            _httpServer.Handler = ctx =>
            {
                // capture request data
                actualHttpMethod = ctx.Request.Method;
                actualRelativePath = ctx.Request.Path.Value;
                actualQueryString = ctx.Request.QueryString.Value;

                return Task.CompletedTask;
            };

            // Act
            await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Equal(command.Request.Method.Method, actualHttpMethod, StringComparer.InvariantCultureIgnoreCase);
            Assert.Equal(command.Request.Url.AbsolutePath, actualRelativePath);
            Assert.Equal(command.Request.Url.Query, actualQueryString);
        }

        [Theory]
        [MemberData(nameof(OpenHttpServerTests.HttpStatusCodes), MemberType = typeof(OpenHttpServerTests))]
        public async Task ResponseCatpured(HttpStatusCode statusCode)
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = _httpServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            _httpServer.Handler = ctx =>
            {
                // set response status code
                ctx.Response.StatusCode = (int)statusCode;

                return Task.CompletedTask;
            };

            // Act
            var @event = await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Equal(statusCode, @event.Response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task RequestTiming(int millisecondsDelay)
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = _httpServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            _httpServer.Handler = async ctx =>
            {
                await Task.Delay(millisecondsDelay);
            };

            // Act
            var expectedStartTime = DateTime.UtcNow;
            var @event = await _executor.CheckHttpEndpointAsync(command);
            var expectedEndTime = DateTime.UtcNow;

            // Assert
            AssertDateTime.Equal(expectedStartTime, @event.RequestTiming.StartTime, TimeSpanComparer.DefaultTolerance);
            AssertDateTime.Equal(expectedEndTime, @event.RequestTiming.EndTime, TimeSpanComparer.DefaultTolerance);
        }

        [Fact]
        public async Task EndpointUnavailableReturnsError()
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = new Uri("http://localhost:9485/"), // nothing should be open on port
                    Method = HttpMethod.Get
                }
            };

            // Act
            var @event = await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Null(@event.Response);
            Assert.Equal("A connection with the server could not be established", @event.ErrorMessage);
        }

        [Fact]
        public async Task ForceCloseConnectionReturnsError()
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = _httpServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            _httpServer.Handler = async ctx =>
            {
                await _httpServer.Host.StopAsync();
            };

            // Act
            var @event = await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Null(@event.Response);
            Assert.Equal("The server returned an invalid or unrecognized response", @event.ErrorMessage);
        }

        [Fact]
        public async Task HangRequestReturnsTimeoutError()
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = _httpServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            // set timeout on client level
            _httpClient.Timeout = TimeSpan.FromMilliseconds(100);

            _httpServer.Handler = async ctx =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            };

            // Act
            var @event = await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Null(@event.Response);
            Assert.Equal("Request timed out", @event.ErrorMessage);
        }

        [Fact]
        public async Task DoNotReadEntireResponseBody()
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = _httpServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            _httpServer.Handler = async ctx =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Accepted;

                // write infinitely to the response stream
                while (true)
                {
                    await ctx.Response.WriteAsync(Guid.NewGuid().ToString());
                }
            };

            // Act
            var @event = await _executor.CheckHttpEndpointAsync(command);

            // Assert
            Assert.Null(@event.ErrorMessage);
            Assert.Equal(HttpStatusCode.Accepted, @event.Response.StatusCode);
        }
    }
}
