﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        [MemberData(nameof(HttpRequestMethods))]
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
                // capture request data
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

        [Theory]
        [MemberData(nameof(HttpStatusCodes))]
        public async Task ResponseCatpured(HttpStatusCode statusCode)
        {
            // Arrange
            var command = new CheckHttpEndpoint()
            {
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Url = _testServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            AnonymousStartup.RequestDelegate = ctx =>
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
                    Url = _testServer.BaseAddress,
                    Method = HttpMethod.Get
                }
            };

            AnonymousStartup.RequestDelegate = async ctx =>
            {
                await Task.Delay(millisecondsDelay);
            };

            // Act
            var expectedStartTime = DateTime.UtcNow;
            var @event = await _executor.CheckHttpEndpointAsync(command);
            var expectedEndTime = DateTime.UtcNow;

            // Assert
            var startTimeDiff = Math.Abs(expectedStartTime.Subtract(@event.RequestTiming.StartTime).TotalMilliseconds);
            var endTimeDiff = Math.Abs(expectedEndTime.Subtract(@event.RequestTiming.EndTime).TotalMilliseconds);
            Assert.True(startTimeDiff < 5);
            Assert.True(endTimeDiff < 5);
        }

        private class AnonymousStartup
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

        private static IEnumerable<object[]> HttpRequestMethods()
        {
            yield return new object[] { HttpMethods.Connect };
            yield return new object[] { HttpMethods.Put };
            yield return new object[] { HttpMethods.Post };
            yield return new object[] { HttpMethods.Patch };
            yield return new object[] { HttpMethods.Trace };
            yield return new object[] { HttpMethods.Head };
            yield return new object[] { HttpMethods.Get };
            yield return new object[] { HttpMethods.Delete };
            yield return new object[] { HttpMethods.Options };
        }

        private static IEnumerable<object[]> HttpStatusCodes()
        {
            foreach (var value in Enum.GetValues(typeof(HttpStatusCode)).Cast<HttpStatusCode>())
            {
                yield return new object[] { value };
            }
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }
}
