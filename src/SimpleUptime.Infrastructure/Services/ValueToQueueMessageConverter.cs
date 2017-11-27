using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SimpleUptime.Infrastructure.Services
{
    public class ValueToQueueMessageConverter
    {
        public CloudQueueMessage Convert(object value)
        {
            var json = JsonConvert.SerializeObject(value, Constants.JsonSerializerSettings);

            return new CloudQueueMessage(json);
        }
    }
}