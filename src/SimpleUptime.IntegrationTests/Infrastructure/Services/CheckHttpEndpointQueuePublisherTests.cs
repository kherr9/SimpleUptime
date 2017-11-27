using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.Services;
using Xunit;

namespace SimpleUptime.IntegrationTests.Infrastructure.Services
{
    public class CheckHttpEndpointQueuePublisherTests : IDisposable
    {
        private readonly CheckHttpEndpointQueuePublisher _publisher;
        private readonly CloudQueue _queue;

        public CheckHttpEndpointQueuePublisherTests()
        {
            var connectionString = "UseDevelopmentStorage=true";

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();

            _queue = queueClient.GetQueueReference(nameof(CheckHttpEndpointQueuePublisherTests).ToLowerInvariant());

            _queue.CreateIfNotExistsAsync().Wait();

            _publisher = new CheckHttpEndpointQueuePublisher(_queue);
        }

        public void Dispose()
        {
            _queue?.DeleteIfExistsAsync().Wait();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task PublishAsyncManyMessages(int count)
        {
            // Arrange
            var commands = GenerateCheckHttpEndpoint(count);

            // Act
            await _publisher.PublishAsync(commands);

            // Assert
            var messages = (await _queue.GetMessagesAsync(count + 1)).ToArray();

            Assert.Equal(messages.Length, count);
        }

        private IEnumerable<CheckHttpEndpoint> GenerateCheckHttpEndpoint(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new CheckHttpEndpoint()
                {
                    HttpMonitorId = HttpMonitorId.Create(),
                    Request = new HttpRequest()
                    {
                        Method = HttpMethod.Get,
                        Url = new Uri("http://example.com/asdfasdfasdfasdfasdf/asdfasdfasdf/asdfasdfasdf/asdfasdf/sadf")
                    }
                };
            }
        }
    }
}
