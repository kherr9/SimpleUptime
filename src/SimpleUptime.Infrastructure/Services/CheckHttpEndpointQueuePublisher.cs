using System;
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
        private readonly CreateCloudQueueAsync _queueFactoryAsync;
        private readonly ValueToQueueMessageConverter _converter = new ValueToQueueMessageConverter();

        public CheckHttpEndpointQueuePublisher(CreateCloudQueueAsync queueFactoryAsync)
        {
            _queueFactoryAsync = queueFactoryAsync;
        }

        public async Task PublishAsync(IEnumerable<CheckHttpEndpoint> commands)
        {
            if (commands == null) throw new ArgumentNullException(nameof(commands));

            var messages = ToMessage(commands);

            var queue = await _queueFactoryAsync("commands");

            foreach (var message in messages)
            {
                await queue.AddMessageAsync(message);
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