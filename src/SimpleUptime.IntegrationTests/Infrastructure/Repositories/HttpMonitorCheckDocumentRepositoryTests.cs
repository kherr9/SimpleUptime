using System;
using System.Net.Http;
using System.Threading.Tasks;
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

        #region Put

        [Fact]
        public async Task PutThrowsExceptionWhenEntityIsNull()
        {
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.PutAsync(null));

            // Assert
            Assert.Equal("httpMonitorCheck", exception.ParamName);
        }

        [Fact]
        public async Task PutCreatesEntity()
        {
            // Arrange
            var entity = GenerateHttpMonitorCheck();

            // Act
            await _repository.PutAsync(entity);

            // Assert
        }

        #endregion

        private HttpMonitorCheck GenerateHttpMonitorCheck()
        {
            return new HttpMonitorCheck()
            {
                Id = HttpMonitorCheckId.Create(),
                HttpMonitorId = HttpMonitorId.Create(),
                Request = new HttpRequest() { Method = HttpMethod.Delete, Url = new Uri("http://yahoo.com") }
            };
        }
    }
}
