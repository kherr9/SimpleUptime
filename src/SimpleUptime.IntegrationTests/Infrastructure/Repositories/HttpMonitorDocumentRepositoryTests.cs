using System;
using System.Linq;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.IntegrationTests.Fixtures;
using Xunit;

namespace SimpleUptime.IntegrationTests.Infrastructure.Repositories
{
    public class HttpMonitorDocumentRepositoryTests : IClassFixture<DocumentDbFixture>, IDisposable
    {
        private readonly DocumentDbFixture _fixture;
        private readonly HttpMonitorDocumentRepository _repository;

        public HttpMonitorDocumentRepositoryTests(DocumentDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new HttpMonitorDocumentRepository(fixture.DocumentClient);
        }

        public void Dispose()
        {
            _fixture.Reset();
        }

        #region Get

        [Fact]
        public async Task GetReturnsEmptySetWhenNoDocumentsExist()
        {
            // Act
            var entities = await _repository.GetAsync();

            // Assert
            Assert.Empty(entities);
        }

        [Fact]
        public async Task GetReturnsSingleEntity()
        {
            // Assert
            var entity = await GenerateAndPersistHttpMonitorAsync();

            // Act
            var entities = await _repository.GetAsync();

            // Assert
            var readEntity = entities.Single();
            Assert.Equal(entity.Id, readEntity.Id);
            Assert.Equal(entity.Url, readEntity.Url);
        }

        [Fact]
        public async Task GetReturnsManyEntity()
        {
            // Assert
            var entities = new[]
            {
                await GenerateAndPersistHttpMonitorAsync(),
                await GenerateAndPersistHttpMonitorAsync(),
                await GenerateAndPersistHttpMonitorAsync()
            };

            // Act
            var readEntities = await _repository.GetAsync();

            // Assert
            Assert.Equal(
                entities.Select(x => x.Id).OrderBy(x => x),
                readEntities.Select(x => x.Id).OrderBy(x => x)
                );
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetByIdReturnsEntity()
        {
            // Arrange
            var existingEntity = await GenerateAndPersistHttpMonitorAsync();

            // Act
            var entity = await _repository.GetByIdAsync(existingEntity.Id);

            // Assert
            Assert.Equal(existingEntity.Id, entity.Id);
        }

        [Fact]
        public async Task GetByIdReturnsNullWhenNotFound()
        {
            // Arrange
            var id = HttpMonitorId.Create();

            // Act
            var entity = await _repository.GetByIdAsync(id);

            // Assert
            Assert.Null(entity);
        }

        #endregion

        #region Put

        [Fact]
        public async Task PutThrowsExceptionWhenEntityIsNull()
        {
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.PutAsync(null));

            // Assert
            Assert.Equal("httpMonitor", exception.ParamName);
        }

        [Fact]
        public async Task PutCreatesEntity()
        {
            // Arrange
            var entity = GenerateHttpMonitor();

            // Act
            await _repository.PutAsync(entity);

            // Assert
            var readEntity = await _repository.GetByIdAsync(entity.Id);
            Assert.Equal(readEntity.Id, entity.Id);
        }

        [Fact]
        public async Task PutUpdatesEntity()
        {
            // Arrange
            var entity = await GenerateAndPersistHttpMonitorAsync();
            var newUrl = new Uri("http://foo-bar.com/");
            entity.Url = newUrl;

            // Act
            await _repository.PutAsync(entity);

            // Assert
            var readEntity = await _repository.GetByIdAsync(entity.Id);
            Assert.Equal(newUrl, readEntity.Url);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteRemovesEntityWhenFound()
        {
            // Arrange
            var entity = await GenerateAndPersistHttpMonitorAsync();

            // Act
            await _repository.DeleteAsync(entity.Id);

            // Assert
            var readEntity = await _repository.GetByIdAsync(entity.Id);
            Assert.Null(readEntity);
        }

        [Fact]
        public async Task DeleteDoesNotThrowExceptionWhenNotFound()
        {
            // Arrange
            var id = HttpMonitorId.Create();

            // Act
            // Assert no exception thrown
            await _repository.DeleteAsync(id);
        }

        #endregion

        private HttpMonitor GenerateHttpMonitor()
        {
            return new HttpMonitor(HttpMonitorId.Create(), new Uri("https://example.com"));
        }

        private async Task<HttpMonitor> GenerateAndPersistHttpMonitorAsync()
        {
            var httpMonitor = GenerateHttpMonitor();

            await _repository.PutAsync(httpMonitor);

            return httpMonitor;
        }
    }
}
