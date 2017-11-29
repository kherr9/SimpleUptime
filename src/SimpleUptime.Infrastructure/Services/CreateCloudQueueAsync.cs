using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SimpleUptime.Infrastructure.Services
{
    public delegate Task<CloudQueue> CreateCloudQueueAsync(string queueName);
}