using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.IntegrationTests.Fixtures;
using SimpleUptime.IntegrationTests.WebApi.Controllers.Client;
using Xunit;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers
{
    public class HttpMonitorsControllerTests : IClassFixture<WebApiAppFixture>, IDisposable
    {
        private readonly WebApiAppFixture _fixture;
        private readonly HttpMonitorClient _client;

        public HttpMonitorsControllerTests(WebApiAppFixture fixture)
        {
            _fixture = fixture;

            _client = new HttpMonitorClient(fixture.HttpClient);
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
            var entities = await GenerateAndPostHttpMonitorAsync(count);

            // Act
            (var response, var model) = await _client.GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(count, model.Length);
            Assert.Equal(entities.OrderBy(x => x.Id), model.OrderBy(x => x.Id));
        }

        #endregion

        #region GetById(id)

        [Fact]
        public async Task GetById()
        {
            // Arrange
            var entity = await GenerateAndPostHttpMonitorAsync();

            // Act
            (var response, var model) = await _client.GetAsync(entity.Id);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(entity, model);
        }

        [Fact]
        public async Task GetByIdReturnsNotFound()
        {
            // Arrange
            var entityId = HttpMonitorId.Create().ToString();

            // Act
            (var response, _) = await _client.GetAsync(entityId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpMonitorIds))]
        public async Task GetByIdReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            (var response, _) = await _client.GetAsync(id.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Post

        [Fact]
        public async Task Post()
        {
            // Arrange
            var entity = (dynamic)Generate();

            // Act
            (var response, var postEntity) = await _client.PostAsync((object)entity);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(entity.Url, postEntity.Url);
            Assert.NotNull(postEntity.Id);
            Assert.NotEmpty(postEntity.Id);
        }

        [Fact]
        public async Task PostGeneratesUniqueId()
        {
            // Arrange
            var entity = Generate();

            // Act
            (_, var postEntity1) = await _client.PostAsync(entity);
            (_, var postEntity2) = await _client.PostAsync(entity);

            // Assert
            Assert.NotEqual(postEntity1.Id, postEntity2.Id);
        }

        #endregion

        #region Put

        [Fact]
        public async Task Put()
        {
            // Arrange
            var entity = await GenerateAndPostHttpMonitorAsync();
            var newUrl = new Uri("https://asdfasdf.example.com");
            entity.Url = newUrl;

            // Act
            (var response, var model) = await _client.PutAsync(entity.Id, entity);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(newUrl, model.Url);
        }

        [Fact]
        public async Task PutReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entity = Generate();
            var id = HttpMonitorId.Create().ToString();

            // Act
            (var response, _) = await _client.PutAsync(id, entity);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpMonitorIds))]
        public async Task PutReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Arrange
            var entity = Generate();

            // Act
            (var response, _) = await _client.PutAsync(id.ToString(), entity);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var entity = await GenerateAndPostHttpMonitorAsync();

            // Act
            var response = await _client.DeleteAsync(entity.Id);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entityId = HttpMonitorId.Create().ToString();

            // Act
            var response = await _client.DeleteAsync(entityId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpMonitorIds))]
        public async Task DeleteReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            var response = await _client.DeleteAsync(id.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        private object Generate()
        {
            return new
            {
                Url = new Uri($"https://{DateTime.UtcNow.Ticks}.example.com/")
            };
        }

        private async Task<HttpMonitorDto> GenerateAndPostHttpMonitorAsync()
        {
            var entity = Generate();

            return (await _client.PostAsync(entity)).Item2;
        }

        private async Task<HttpMonitorDto[]> GenerateAndPostHttpMonitorAsync(int count)
        {
            var entities = new List<HttpMonitorDto>();
            for (var i = 0; i < count; i++)
            {
                var entity = await GenerateAndPostHttpMonitorAsync();
                entities.Add(entity);
            }

            return entities.ToArray();
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
    }
}
