using System;
using System.Threading.Tasks;
using SimpleUptime.Domain.Events;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Infrastructure.Services
{
    public class HttpMonitorCheckedQueuePublisher : IHttpMonitorCheckedPublisher
    {
        private readonly CreateCloudQueueAsync _queueFactoryAsync;
        private readonly ValueToQueueMessageConverter _converter = new ValueToQueueMessageConverter();
        private readonly string[] _queueNames = new[]
        {
            $"events-{nameof(HttpMonitorChecked)}-{nameof(HttpMonitor)}".ToLowerInvariant()
        };

        public HttpMonitorCheckedQueuePublisher(CreateCloudQueueAsync queueFactoryAsync)
        {
            _queueFactoryAsync = queueFactoryAsync;
        }

        public async Task PublishAsync(HttpMonitorChecked @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            // todo: can i resuse message?
            var message = _converter.Convert(@event);

            foreach (var queueName in _queueNames)
            {
                var queue = await _queueFactoryAsync(queueName);

                await queue.AddMessageAsync(message);
            }
        }
    }
}
