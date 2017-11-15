using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using SimpleUptime.Application.Services;
using SimpleUptime.WebApi;
using Xunit;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers
{
    public class HttpMonitorsControllerTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HttpMonitorsControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetWhenNoneReturnsEmpty()
        {
            // Act
            var result = await _client.GetAsync(Urls.HttpMonitors.Get());

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var body = await result.Content.ReadAsStringAsync();
            Assert.Equal("[]", body);
        }

        [Fact]
        public async Task GetWhenSingle()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();

            // Act
            var response = await _client.GetAsync(Urls.HttpMonitors.Get());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal($"[{{\"\"Id:\"{entity.Id}\"}}]", body);
        }

        [Fact]
        public async Task GetByIdReturnsNotFound()
        {
            // Arrange
            var entityId = "some_id";

            // Act
            var response = await _client.GetAsync(Urls.HttpMonitors.Get(entityId));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();

            // Act
            var response = await _client.GetAsync(Urls.HttpMonitors.Get(entity.Id));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal($"{{\"\"Id:\"{entity.Id}\"}}", body);
        }

        [Fact]
        public async Task Post()
        {
            // Arrange
            var entity = Generate();

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Urls.HttpMonitors.Post(), content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var postEntity = await response.Content.ReadAsJsonAsync<HttpMonitorDto>();
            Assert.Equal(entity.Url, postEntity.Url);
            Assert.NotNull(postEntity.Id);
            Assert.NotEmpty(postEntity.Id);
        }

        [Fact]
        public async Task Put()
        {
            // Arrange
            var entity = Generate();

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Urls.HttpMonitors.Post(), content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var postEntity = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Assert.Equal(entity.Url, postEntity.Url);
            Assert.IsType<Guid>(postEntity.Id);
            Assert.NotEqual(Guid.Empty, postEntity.Id);
        }

        private dynamic Generate()
        {
            return new
            {
                Url = $"https://{DateTime.UtcNow.Ticks}.example.com/"
            };
        }

        private Task<dynamic> GenerateAndPersistEntityAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
        }
    }

    public static class HttpContentExtension
    {
        public static async Task<TModel> ReadAsJsonAsync<TModel>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TModel>(json);
        }
    }
}
