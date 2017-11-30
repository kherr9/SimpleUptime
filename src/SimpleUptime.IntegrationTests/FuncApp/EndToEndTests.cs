using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.IntegrationTests.Fixtures;
using Xunit;

namespace SimpleUptime.IntegrationTests.FuncApp
{
    public class EndToEndTests : IClassFixture<FuncAppFixture>
    {
        private readonly FuncAppFixture _fixture;
        private readonly IHttpMonitorRepository _httpMonitorRepository;
        private readonly IHttpMonitorCheckRepository _httpMonitorCheckRepository;
        private readonly OpenHttpServer _openHttpServer;

        public EndToEndTests(FuncAppFixture fixture)
        {
            _fixture = fixture;
            _httpMonitorRepository = fixture.HttpMonitorRepository;
            _httpMonitorCheckRepository = fixture.HttpMonitorCheckRepository;
            _openHttpServer = fixture.OpenHttpServer;
        }

        [Fact]
        public async Task TestLaunch()
        {
            // Arrange
            var httpMonitor1 = new HttpMonitor(
                HttpMonitorId.Create(),
                new HttpRequest(HttpMethod.Get, _fixture.OpenHttpServer.BaseAddress));

            var httpMonitor2 = new HttpMonitor(
                HttpMonitorId.Create(),
                new HttpRequest(HttpMethod.Delete, _fixture.OpenHttpServer.BaseAddress));

            await _httpMonitorRepository.PutAsync(httpMonitor1);
            await _httpMonitorRepository.PutAsync(httpMonitor2);

            var tcs1 = new TaskCompletionSource<object>();
            var tcs2 = new TaskCompletionSource<object>();
            var combinedTasks = Task.WhenAll(tcs1.Task, tcs2.Task);
            _openHttpServer.Handler = ctx =>
            {
                if (string.Equals(ctx.Request.Method, httpMonitor1.Request.Method.Method,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    tcs1.SetResult(null);
                    ctx.Response.StatusCode = 200;
                }
                else if (string.Equals(ctx.Request.Method, httpMonitor2.Request.Method.Method,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    tcs2.SetResult(null);
                    ctx.Response.StatusCode = 300;
                }
                else
                {
                    ctx.Response.StatusCode = 404;
                }

                return Task.CompletedTask;
            };

            // Act
            await _fixture.StartHostAsync();

            await Task.WhenAny(
                combinedTasks,
                Task.Delay(10000));

            // Assert
            Assert.True(combinedTasks.IsCompletedSuccessfully);

            // todo race condition
            await Task.Delay(100);

            var check1 = (await _httpMonitorCheckRepository.GetAsync(httpMonitor1.Id)).SingleOrDefault();
            Assert.NotNull(check1);
            Assert.Equal(200, (int)check1.Response.StatusCode);

            var check2 = (await _httpMonitorCheckRepository.GetAsync(httpMonitor2.Id)).SingleOrDefault();
            Assert.NotNull(check2);
            Assert.Equal(300, (int)check2.Response.StatusCode);
        }
    }
}
