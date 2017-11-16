using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
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
            var entityId = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync(Urls.HttpMonitors.Get(entityId));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task GetByIdReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            var response = await _client.GetAsync(Urls.HttpMonitors.Get(id.ToString()));

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
            var readEntity = await response.Content.ReadAsJsonAsync<HttpMonitorDto>();
            Assert.NotNull(readEntity);
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
        public async Task PutUpdatesUrl()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();
            var newUrl = new Uri("https://asdfasdf.example.com");
            entity.Url = newUrl;

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(Urls.HttpMonitors.Put(entity.Id), content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var postEntity = JsonConvert.DeserializeObject<HttpMonitorDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(entity.Id, postEntity.Id);
            Assert.Equal(newUrl, postEntity.Url);
        }

        [Fact]
        public async Task PutReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entity = Generate();
            var id = Guid.NewGuid().ToString();

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(Urls.HttpMonitors.Put(id), content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task PutReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Arrange
            var entity = Generate();

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(Urls.HttpMonitors.Put(id.ToString()), content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();

            // Act
            var response = await _client.DeleteAsync(Urls.HttpMonitors.Delete(entity.Id));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entityId = Guid.NewGuid().ToString();

            // Act
            var response = await _client.DeleteAsync(Urls.HttpMonitors.Delete(entityId));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task DeleteReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            var response = await _client.DeleteAsync(Urls.HttpMonitors.Delete(id.ToString()));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private dynamic Generate()
        {
            return new
            {
                Url = new Uri($"https://{DateTime.UtcNow.Ticks}.example.com/")
            };
        }

        private async Task<HttpMonitorDto> GenerateAndPersistEntityAsync()
        {
            var entity = Generate();

            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync(Urls.HttpMonitors.Post(), content))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsJsonAsync<HttpMonitorDto>();
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
        }
    }
}
