using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.Infrastructure.Services;
using SimpleUptime.IntegrationTests.Fixtures;
using SimpleUptime.IntegrationTests.Infrastructure.Services;
using Xunit;

namespace SimpleUptime.IntegrationTests.Application.Services
{
    public class CheckHttpMonitorPublisherServiceTests : IClassFixture<DocumentDbFixture>, IDisposable
    {
        private readonly DocumentDbFixture _fixture;
        private readonly CheckHttpMonitorPublisherService _service;
        private readonly IHttpMonitorRepository _repository;
        private readonly ICheckHttpEndpointPublisher _publisher;
        private readonly CloudQueue _queue;

        public CheckHttpMonitorPublisherServiceTests(DocumentDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new HttpMonitorDocumentRepository(fixture.DocumentClient, DatabaseConfigurations.Create());

            var connectionString = "UseDevelopmentStorage=true";

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();

            _queue = queueClient.GetQueueReference(nameof(CheckHttpEndpointQueuePublisherTests).ToLowerInvariant());

            _queue.CreateIfNotExistsAsync().Wait();

            _publisher = new CheckHttpEndpointQueuePublisher(name => Task.FromResult(_queue));

            _service = new CheckHttpMonitorPublisherService(_repository, _publisher);
        }

        public void Dispose()
        {
            _fixture.Reset();
            _queue.DeleteIfExistsAsync().Wait();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task Foo(int count)
        {
            // Arrange
            await GenerateAsync(count);

            // Act
            await _service.PublishAsync();

            // Assert
            var messages = await GetAllQueueMessageAsync();
            Assert.Equal(count, messages.Count);
        }

        private async Task GenerateAsync(int count)
        {
            for (var i = 0; i < count; i++)
            {
                await GenerateAsync();
            }
        }

        private Task GenerateAsync()
        {
            var httpMonitor = new HttpMonitor()
            {
                Id = HttpMonitorId.Create(),
                Request = new HttpRequest()
                {
                    Method = HttpMethod.Get,
                    Url = new Uri("http://example.com/sdfsd/sdfsdfsd/sdffsd")
                }
            };

            return _repository.PutAsync(httpMonitor);
        }

        private async Task<ICollection<CloudQueueMessage>> GetAllQueueMessageAsync()
        {
            var messages = new List<CloudQueueMessage>();

            while (true)
            {
                var batch = (await _queue.GetMessagesAsync(32)).ToList();

                if (batch.Any())
                {
                    messages.AddRange(batch);
                }
                else
                {
                    return messages;
                }
            }
        }
    }
}
