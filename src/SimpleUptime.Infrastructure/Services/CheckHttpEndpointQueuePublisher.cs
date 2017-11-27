using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using MoreLinq;
using Newtonsoft.Json;
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

    public class CheckHttpEndpointBatchQueuePublisher : ICheckHttpEndpointPublisher
    {
        private readonly CloudQueue _client;
        private readonly ValueToQueueMessageConverter _converter = new ValueToQueueMessageConverter();

        public CheckHttpEndpointBatchQueuePublisher(CloudQueue client)
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

        ////private IEnumerable<CloudQueueMessage> ToMessage(IEnumerable<CheckHttpEndpoint> commands)
        ////{
        ////    foreach (var batch in commands.Batch(250))
        ////    {
        ////        var jsons = batch.Select(x => JsonConvert.SerializeObject(x, Constants.JsonSerializerSettings));

        ////        var json = string.Join(",", jsons);

        ////        yield return new CloudQueueMessage(json);
        ////    }
        ////}

        private IEnumerable<CloudQueueMessage> ToMessage(IEnumerable<CheckHttpEndpoint> commands)
        {
            // todo: string builder, https://msdn.microsoft.com/en-us/library/w3739zdy(v=vs.110).aspx
            const int messageLimitInBytes = 1000 * 64;
            using (var builder = new MemoryStream())
            {
                foreach (var command in commands)
                {
                    var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command, Constants.JsonSerializerSettings));

                    if (buffer.Length + builder.Length > messageLimitInBytes)
                    {
                        // flush
                        yield return new CloudQueueMessage(builder.ToArray());
                        builder.SetLength(0);
                    }

                    builder.Write(buffer, 0, buffer.Length);
                }

                if (builder.Length > 0)
                {
                    // flush
                    yield return new CloudQueueMessage(builder.ToArray());
                }
            }
        }

        private CloudQueueMessage ToMessage(CheckHttpEndpoint command)
        {
            var message = _converter.Convert(command);

            return message;
        }
    }
}