using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using SimpleUptime.Domain.Events;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Infrastructure.Services
{
    public class HttpEndpointCheckedQueuePublisher : IHttpEndpointCheckedPublisher
    {
        private readonly CreateCloudQueueAsync _queueFactoryAsync;
        private readonly ValueToQueueMessageConverter _converter = new ValueToQueueMessageConverter();

        public HttpEndpointCheckedQueuePublisher(CreateCloudQueueAsync queueFactoryAsync)
        {
            _queueFactoryAsync = queueFactoryAsync;
        }

        public async Task PublishAsync(HttpEndpointChecked @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            var message = ToMessage(@event);

            var queue = await _queueFactoryAsync("events");

            await queue.AddMessageAsync(message);
        }

        private CloudQueueMessage ToMessage(HttpEndpointChecked @event)
        {
            var message = _converter.Convert(@event);

            return message;
        }
    }
}