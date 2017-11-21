using System;
using System.Net;
using System.Net.Http;
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
        private readonly Uri _baseUrl = new Uri("http://localhost:5000");

        public HttpMonitorTestControllerTests(WebApiAppFixture webApiAppFixture)
        {
            _webApiAppFixture = webApiAppFixture;
            _client = new HttpMonitorClient(webApiAppFixture.HttpClient);

            _testServer = DummyHttpTestServer.CreateAndRunWebHost(_baseUrl.ToString());
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
                Url = new Uri(_baseUrl, "foo/bar?q=123")
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
             (var response, var testResult) = await _client.TestAsync(entity.Id);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("GET", actualMethod, StringComparer.InvariantCultureIgnoreCase);
            Assert.Equal(entity.Url.AbsolutePath, actualPath);
            Assert.Equal(entity.Url.Query, actualQueryString);

            var expectedResult = new HttpMonitorCheckedDto()
            {
                HttpMonitorId = entity.Id,
                Request = new HttpRequestDto()                                 
                {
                    Url = entity.Url,
                    Method = HttpMethod.Get.ToString()
                },
                Response = new HttpResponseDto()
                {                              
                    StatusCode = (int)HttpStatusCode.OK
                },
                RequestTiming = new HttpRequestTimingDto()
                {
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow
                }
            };

            Assert.Equal(expectedResult.HttpMonitorId, testResult.HttpMonitorId);
            Assert.Equal(expectedResult.Request, testResult.Request);
            Assert.Equal(expectedResult.Response, testResult.Response);

            AssertDateTime.Equal(expectedResult.RequestTiming.StartTime, testResult.RequestTiming.StartTime, TimeSpanComparer.DefaultTolerance);
            AssertDateTime.Equal(expectedResult.RequestTiming.EndTime, testResult.RequestTiming.EndTime, TimeSpanComparer.DefaultTolerance);
        }
    }
}
