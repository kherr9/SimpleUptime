using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleUptime.IntegrationTests.Fixtures;
using Xunit;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers
{
    public class HttpMonitorsControllerTests : IClassFixture<WebApiAppFixture>, IDisposable
    {
        private readonly WebApiAppFixture _fixture;
        private readonly HttpClient _client;

        public HttpMonitorsControllerTests(WebApiAppFixture fixture)
        {
            _fixture = fixture;

            _client = fixture.HttpClient;
        }

        public void Dispose()
        {
            _fixture.Reset();
        }

        #region Get

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public async Task Get(int count)
        {
            // Arrange
            var entities = new List<HttpMonitorDto>();
            for (var i = 0; i < count; i++)
            {
                var entity = await GenerateAndPersistEntityAsync();
                entities.Add(entity);
            }

            // Act
            var result = await GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.Equal(count, result.Model.Length);
            Assert.Equal(entities.OrderBy(x => x.Id), result.Model.OrderBy(x => x.Id));
        }

        #endregion

        #region GetById(id)

        [Fact]
        public async Task GetById()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();

            // Act
            var result = await GetAsync(entity.Id);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.Equal(entity, result.Model);
        }

        [Fact]
        public async Task GetByIdReturnsNotFound()
        {
            // Arrange
            var entityId = Guid.NewGuid().ToString();

            // Act
            var result = await GetAsync(entityId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpMonitorIds))]
        public async Task GetByIdReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            var result = await GetAsync(id.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
        }

        #endregion

        #region Post

        [Fact]
        public async Task Post()
        {
            // Arrange
            var entity = Generate();

            // Act
            var postEntity1 = await PostAsync((object)entity);

            // Assert
            Assert.Equal(HttpStatusCode.OK, postEntity1.HttpResponseMessage.StatusCode);
            Assert.Equal(entity.Url, postEntity1.Model.Url);
            Assert.NotNull(postEntity1.Model.Id);
            Assert.NotEmpty(postEntity1.Model.Id);
        }

        [Fact]
        public async Task PostGeneratesUniqueId()
        {
            // Arrange
            var entity = (object)Generate();

            // Act
            var postEntity1 = await PostAsync(entity);
            var postEntity2 = await PostAsync(entity);

            // Assert
            Assert.NotEqual(postEntity1.Model.Id, postEntity2.Model.Id);
        }

        #endregion

        #region Put

        [Fact]
        public async Task Put()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();
            var newUrl = new Uri("https://asdfasdf.example.com");
            entity.Url = newUrl;

            // Act
            var result = await PutAsync(entity.Id, entity);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.Equal(entity.Id, result.Model.Id);
            Assert.Equal(newUrl, result.Model.Url);
        }

        [Fact]
        public async Task PutReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entity = (object)Generate();
            var id = Guid.NewGuid().ToString();

            // Act
            var result = await PutAsync(id, entity);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpMonitorIds))]
        public async Task PutReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Arrange
            var entity = (object)Generate();

            // Act
            var result = await PutAsync(id.ToString(), entity);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var entity = await GenerateAndPersistEntityAsync();

            // Act
            var result = await DeleteAsync(entity.Id);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, result.HttpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task DeleteReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entityId = Guid.NewGuid().ToString();

            // Act
            var result = await DeleteAsync(entityId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpMonitorIds))]
        public async Task DeleteReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            var response = await _client.DeleteAsync(Urls.HttpMonitors.Delete(id.ToString()));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

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

            return (await PostAsync((object)entity)).Model;
        }

        private static IEnumerable<object[]> InvalidHttpMonitorIds()
        {
            yield return new object[] { "foo" };
            yield return new object[] { 1 };
            yield return new object[] { 0 };
            yield return new object[] { -1 };
            yield return new object[] { int.MaxValue };
            yield return new object[] { long.MaxValue };
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { Guid.Empty.ToString() };
        }

        private async Task<ResponseEnvelope<HttpMonitorDto[]>> GetAsync()
        {
            var response = await _client.GetAsync(Urls.HttpMonitors.Get());

            return await ResponseEnvelope<HttpMonitorDto[]>.CreateAsync(response);
        }

        private async Task<ResponseEnvelope<HttpMonitorDto>> GetAsync(string id)
        {
            var response = await _client.GetAsync(Urls.HttpMonitors.Get(id));

            return await ResponseEnvelope<HttpMonitorDto>.CreateAsync(response);
        }

        private async Task<ResponseEnvelope<HttpMonitorDto>> PostAsync(object entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Urls.HttpMonitors.Post(), content);

            return await ResponseEnvelope<HttpMonitorDto>.CreateAsync(response);
        }

        private async Task<ResponseEnvelope<HttpMonitorDto>> PutAsync(string id, object entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(Urls.HttpMonitors.Put(id), content);

            return await ResponseEnvelope<HttpMonitorDto>.CreateAsync(response);
        }

        private async Task<ResponseEnvelope> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync(Urls.HttpMonitors.Delete(id));

            return await ResponseEnvelope.CreateAsync(response);
        }

        private class ResponseEnvelope
        {
            public HttpResponseMessage HttpResponseMessage { get; private set; }

            public static Task<ResponseEnvelope> CreateAsync(HttpResponseMessage httpResponseMessage)
            {
                return Task.FromResult(new ResponseEnvelope()
                {
                    HttpResponseMessage = httpResponseMessage
                });
            }
        }

        private class ResponseEnvelope<TModel>
        {
            public HttpResponseMessage HttpResponseMessage { get; private set; }

            public TModel Model { get; private set; }

            public static async Task<ResponseEnvelope<TModel>> CreateAsync(HttpResponseMessage httpResponseMessage)
            {
                return new ResponseEnvelope<TModel>()
                {
                    HttpResponseMessage = httpResponseMessage,
                    Model = httpResponseMessage.StatusCode == HttpStatusCode.OK ? await httpResponseMessage.Content.ReadAsJsonAsync<TModel>() : default(TModel)
                };
            }
        }
    }
}
