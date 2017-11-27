using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SimpleUptime.MasterApp.Infrastructure;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.MasterApp
{
    public static class CheckHttpMonitorPublisher
    {
        [FunctionName("CheckHttpMonitorPublisher")]
        public static async Task Run(
            [TimerTrigger("0 * * * * *", RunOnStartup = true)]TimerInfo myTimer,
            TraceWriter log,
            [Inject]ICheckHttpMonitorPublisherService service)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            await service.PublishAsync();
        }

        [FunctionName("WorkQueue")]
        public static async Task ConsumeQueueAsync(
            [QueueTrigger("work")] string json,
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
