using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.IntegrationTests.Fixtures;
using Xunit;

namespace SimpleUptime.IntegrationTests.Infrastructure.Repositories
{
    public class HttpMonitorCheckDocumentRepositoryTests : IClassFixture<DocumentDbFixture>, IDisposable
    {
        private readonly DocumentDbFixture _fixture;
        private readonly HttpMonitorCheckDocumentRepository _repository;

        public HttpMonitorCheckDocumentRepositoryTests(DocumentDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new HttpMonitorCheckDocumentRepository(fixture.DocumentClient, DatabaseConfigurations.Create());
        }

        public void Dispose()
        {
            _fixture.Reset();
        }

        #region Create

        [Fact]
        public async Task CreateCreatesEntity()
        {
            // Arrange
            var entity = GenerateHttpMonitorCheck();

            // Act
            await _repository.CreateAsync(entity);

            // Assert
            var readEntity = await _repository.GetAsync(entity.Id);
            Assert.Equal(readEntity.Id, entity.Id);
        }

        [Fact]
        public async Task CreateThrowsExceptionWhenIdExists()
        {
            // Arrange
            var existingEntityId = (await GenerateAndPersistHttpMonitorCheck()).Id;
            var entity = GenerateHttpMonitorCheck(existingEntityId);

            // Act
            var ex = await Assert.ThrowsAsync<DocumentClientException>(() => _repository.CreateAsync(entity));

            // Assert
            Assert.Equal("Conflict", ex.Error.Code);
            Assert.True(ex.Message.Contains("Resource with specified id or name already exists"), ex.Message);
        }

        [Fact]
        public async Task CreateThrowsExceptionWhenEntityIsNull()
        {
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAsync(null));

            // Assert
            Assert.Equal("httpMonitorCheck", exception.ParamName);
        }

        #endregion

        #region Get(HttpMonitorId)

        [Fact]
        public async Task GetByHttpMonitorIdWhenExists()
        {
            // Arrange
            var entity = await GenerateAndPersistHttpMonitorCheck();

            // Act
            var result = await _repository.GetAsync(entity.HttpMonitorId);

            // Assert
            var readEntity = result.Single();
            Assert.Equal(entity.Id, readEntity.Id);
        }

        #endregion

        private HttpMonitorCheck GenerateHttpMonitorCheck()
        {
            return GenerateHttpMonitorCheck(HttpMonitorCheckId.Create());
        }

        private HttpMonitorCheck GenerateHttpMonitorCheck(HttpMonitorCheckId id)
        {
            return new HttpMonitorCheck(
                id,
                HttpMonitorId.Create(),
                new HttpRequest(HttpMethod.Delete, new Uri("http://yahoo.com")),
                new HttpRequestTiming(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(1)),
                new HttpResponse() { StatusCode = HttpStatusCode.Accepted });
        }

        private async Task<HttpMonitorCheck> GenerateAndPersistHttpMonitorCheck()
        {
            var httpMonitorCheck = GenerateHttpMonitorCheck();

            await _repository.CreateAsync(httpMonitorCheck);

            return httpMonitorCheck;
        }
    }
}
