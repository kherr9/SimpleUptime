using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SimpleUptime.MasterApp.Infrastructure;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Services;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.MasterApp
{
    public static class CheckHttpMonitorPublisher
    {
        [FunctionName("PublishCheckHttpEndpointAsync")]
        public static async Task PublishCheckHttpEndpointAsync(
            [TimerTrigger("0 * * * * *", RunOnStartup = true)]TimerInfo myTimer,
            TraceWriter log,
            [Inject]ICheckHttpMonitorPublisherService service)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            await service.PublishAsync();
        }

        [FunctionName("HandleCheckHttpEndpointAsync")]
        public static async Task HandleCheckHttpEndpointAsync(
            [QueueTrigger("work")] string json,
            TraceWriter log,
            [Inject] IHttpMonitorExecutor executor)
        {
            var check = JsonConvert.DeserializeObject<CheckHttpEndpoint>(json, Constants.JsonSerializerSettings);

            var result = await executor.CheckHttpEndpointAsync(check);
        }
    }
}
