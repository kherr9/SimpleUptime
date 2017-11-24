using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Infrastructure.Services
{
    public class CheckHttpEndpointPublisher : ICheckHttpEndpointPublisher
    {
        private readonly ITopicClient _client;
        private readonly JsonSerializer _serializer;

        public CheckHttpEndpointPublisher(ITopicClient client, JsonSerializer serializer)
        {
            _client = client;
            _serializer = serializer;
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
            var bytes = Serialize(command);

            var message = new Message(bytes)
            {
                ContentType = "application/json",
                MessageId = $"{DateTime.UtcNow:yyyyMMddhhmm}-{command.HttpMonitorId}",
                UserProperties =
                {
                    { "Message-AssemblyQualifiedName", command.GetType().AssemblyQualifiedName }
                }
            };

            return message;
        }

        private byte[] Serialize(CheckHttpEndpoint command)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                using (var writer = new JsonTextWriter(sw))
                {
                    _serializer.Serialize(writer, command);
                }

                return ms.ToArray();
            }
        }
    }
}
