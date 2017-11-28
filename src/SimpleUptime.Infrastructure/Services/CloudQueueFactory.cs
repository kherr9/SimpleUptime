using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SimpleUptime.Infrastructure.Services
{
    public class CloudQueueFactory
    {
        private readonly CloudQueueClient _client;

        public CloudQueueFactory(CloudQueueClient client)
        {
            _client = client;
        }

        public async Task<CloudQueue> CreateCloudQueueAsync(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            var queue = _client.GetQueueReference(queueName);

            await queue.CreateIfNotExistsAsync();

            return queue;
        }
    }
}