using System;
using System.Collections.Generic;
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
    public class CheckHttpEndpointQueuePublisherTests
    {
        private readonly CloudQueue _queue;

        public CheckHttpEndpointQueuePublisherTests()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=simpleuptimedevdata002;AccountKey=YXSPvZfhv17z0GvHoOQGCc5amk0kBXNsZnC4onVfUPsTox3blrNPWTILhwdLBtcDsJHs0foA4RzS7sOYRdSyzA==;EndpointSuffix=core.windows.net";

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();

            _queue = queueClient.GetQueueReference(nameof(CheckHttpEndpointQueuePublisherTests).ToLowerInvariant());

            _queue.CreateIfNotExistsAsync().Wait();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task PublishIsFast(int count)
        {
            // Arrange
            var publisher = new CheckHttpEndpointQueuePublisher(_queue);
            var commands = GenerateCheckHttpEndpoint(count);

            // Act
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await publisher.PublishAsync(commands);
            sw.Stop();

            // Assert
            Console.WriteLine($"Count:{count}, TotalMilliseconds:{sw.ElapsedMilliseconds}, Avg:{Convert.ToDouble(sw.ElapsedMilliseconds) / count}");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task PublishBatchIsFast(int count)
        {
            // Arrange
            var publisher = new CheckHttpEndpointBatchQueuePublisher(_queue);
            var commands = GenerateCheckHttpEndpoint(count);

            // Act
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await publisher.PublishAsync(commands);
            sw.Stop();

            // Assert
            Console.WriteLine($"Count:{count}, TotalMilliseconds:{sw.ElapsedMilliseconds}, Avg:{Convert.ToDouble(sw.ElapsedMilliseconds) / count}");
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
