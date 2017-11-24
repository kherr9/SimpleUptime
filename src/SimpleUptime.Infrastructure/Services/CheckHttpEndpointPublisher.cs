using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Infrastructure.Services
{
    public class CheckHttpEndpointPublisher : ICheckHttpEndpointPublisher
    {
        private readonly ITopicClient _client;
        private readonly ValueToMessageConverter _converter = new ValueToMessageConverter();

        public CheckHttpEndpointPublisher(ITopicClient client)
        {
            _client = client;
        }

        public async Task PublishAsync(IEnumerable<CheckHttpEndpoint> commands)
        {
            var messages = ToMessage(commands);

            foreach (var message in messages)
            {
                await _client.SendAsync(message);
            }
        }

        private IEnumerable<Message> ToMessage(IEnumerable<CheckHttpEndpoint> commands)
        {
            return commands.Select(ToMessage);
        }

        private Message ToMessage(CheckHttpEndpoint command)
        {
            var message = _converter.Convert(command);

            message.MessageId = $"{DateTime.UtcNow:yyyyMMddhhmm}-{command.HttpMonitorId}";

            return message;
        }
    }
}
