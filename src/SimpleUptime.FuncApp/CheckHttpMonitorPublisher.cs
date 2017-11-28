using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Services;
using SimpleUptime.FuncApp.Infrastructure;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.FuncApp
{
    public static class CheckHttpMonitorPublisher
    {
        [FunctionName("PublishCheckHttpEndpointAsync")]
        public static async Task PublishCheckHttpEndpointAsync(
            [TimerTrigger("0 * * * * *", RunOnStartup = true)]TimerInfo myTimer,
            TraceWriter log,
            [Inject]ICheckHttpMonitorPublisherService service)
        {
            log.Info($"Executing {nameof(PublishCheckHttpEndpointAsync)}");

            await service.PublishAsync();
        }

        [FunctionName("HandleCheckHttpEndpointAsync")]
        public static async Task HandleCheckHttpEndpointAsync(
            [QueueTrigger("commands")] string json,
            TraceWriter log,
            [Inject] IHttpMonitorExecutor executor,
            [Inject] IHttpEndpointCheckedPublisher publisher)
        {
            var check = JsonConvert.DeserializeObject<CheckHttpEndpoint>(json, Constants.JsonSerializerSettings);

            var result = await executor.CheckHttpEndpointAsync(check);

            log.Info($"{nameof(HandleCheckHttpEndpointAsync)} {JsonConvert.SerializeObject(result, Constants.JsonSerializerSettings)}");

            await publisher.PublishAsync(result);
        }
    }
}