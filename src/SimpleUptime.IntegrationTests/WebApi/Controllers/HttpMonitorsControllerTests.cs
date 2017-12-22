using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.IntegrationTests.Fixtures;
using SimpleUptime.IntegrationTests.Util.Client;
using SimpleUptime.IntegrationTests.Util.Helpers;
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
        [MemberData(nameof(HttpMonitorIdHelper.InvalidHttpMonitorIds), MemberType = typeof(HttpMonitorIdHelper))]
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
            var entity = (dynamic)EntityGenerator.GenerateHttpMonitor();

            // Act
            (var response, var postEntity) = await _client.PostAsync((object)entity);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(entity.Request.Url, postEntity.Request.Url);
            Assert.Equal(entity.Request.Method, postEntity.Request.Method);
            Assert.NotNull(postEntity.Id);
            Assert.NotEmpty(postEntity.Id);
        }

        [Fact]
        public async Task PostGeneratesUniqueId()
        {
            // Arrange
            var entity = EntityGenerator.GenerateHttpMonitor();

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
            entity.Request.Url = newUrl;

            // Act
            (var response, var model) = await _client.PutAsync(entity.Id, entity);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(newUrl, model.Request.Url);
        }

        [Fact]
        public async Task PutReturnsNotFoundWhenEntityDoesNotExist()
        {
            // Arrange
            var entity = EntityGenerator.GenerateHttpMonitor();
            var id = HttpMonitorId.Create().ToString();

            // Act
            (var response, _) = await _client.PutAsync(id, entity);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(HttpMonitorIdHelper.InvalidHttpMonitorIds), MemberType = typeof(HttpMonitorIdHelper))]
        public async Task PutReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Arrange
            var entity = EntityGenerator.GenerateHttpMonitor();

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
        [MemberData(nameof(HttpMonitorIdHelper.InvalidHttpMonitorIds), MemberType = typeof(HttpMonitorIdHelper))]
        public async Task DeleteReturnsNotFoundWhenIdNotValidFormat(object id)
        {
            // Act
            var response = await _client.DeleteAsync(id.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        private async Task<HttpMonitorDto> GenerateAndPostHttpMonitorAsync()
        {
            var entity = EntityGenerator.GenerateHttpMonitor();

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
    }
}
