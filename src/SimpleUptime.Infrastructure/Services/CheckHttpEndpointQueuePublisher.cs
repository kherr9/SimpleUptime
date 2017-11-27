using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Infrastructure.Services
{
    public class CheckHttpEndpointQueuePublisher : ICheckHttpEndpointPublisher
    {
        private readonly CloudQueue _client;
        private readonly ValueToQueueMessageConverter _converter = new ValueToQueueMessageConverter();

        public CheckHttpEndpointQueuePublisher(CloudQueue client)
        {
            _client = client;
        }

        public async Task PublishAsync(IEnumerable<CheckHttpEndpoint> commands)
        {
            var messages = ToMessage(commands);

            foreach (var message in messages)
            {
                await _client.AddMessageAsync(message);
            }
        }

        private IEnumerable<CloudQueueMessage> ToMessage(IEnumerable<CheckHttpEndpoint> commands)
        {
            return commands.Select(ToMessage);
        }

        private CloudQueueMessage ToMessage(CheckHttpEndpoint command)
        {
            var message = _converter.Convert(command);

            return message;
        }
    }
}