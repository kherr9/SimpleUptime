using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.WorkerApp
{
    public static class Functions
    {
        [FunctionName("HandleCheckHttpMonitorPublisher")]
        public static async Task Run(
            [ServiceBusTrigger("master.events", "worker", AccessRights.Listen, Connection = "azureServiceBus")] BrokeredMessage message,
            TraceWriter log)
        {
            var converter = new BrokeredMessageToValueConverter();
            var check = (CheckHttpEndpoint)converter.Convert(message);

            using (var client = new HttpClient())
            {
                var executor = new HttpMonitorExecutor(client);

                var result = await executor.CheckHttpEndpointAsync(new CheckHttpEndpoint()
                {
                    HttpMonitorId = check.HttpMonitorId,
                    Request = check.Request
                });
            }
        }

        [FunctionName("HandleQueue")]
        public static async Task Run2(
            [QueueTrigger("checkhttpendpoint")] string json,
            TraceWriter log)
        {
            var check = JsonConvert.DeserializeObject<CheckHttpEndpoint>(json, Constants.JsonSerializerSettings);

            using (var client = new HttpClient())
            {
                var executor = new HttpMonitorExecutor(client);

                var result = await executor.CheckHttpEndpointAsync(new CheckHttpEndpoint()
                {
                    HttpMonitorId = check.HttpMonitorId,
                    Request = check.Request
                });
            }
        }
    }
}
