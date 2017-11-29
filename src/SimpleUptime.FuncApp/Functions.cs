using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;
using SimpleUptime.FuncApp.Infrastructure;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.FuncApp
{
    public static class Functions
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

        // todo not idempotent
        [FunctionName("HandleCheckHttpEndpointAsync")]
        public static async Task HandleCheckHttpEndpointAsync(
            [QueueTrigger("commands")] string json,
            TraceWriter log,
            [Inject] IHttpMonitorExecutor executor,
            [Inject] IHttpMonitorCheckRepository repository)
        {
            var check = JsonConvert.DeserializeObject<CheckHttpEndpoint>(json, Constants.JsonSerializerSettings);

            var @event = await executor.CheckHttpEndpointAsync(check);

            log.Info($"{nameof(HandleCheckHttpEndpointAsync)} {JsonConvert.SerializeObject(@event, Constants.JsonSerializerSettings)}");

            await repository.CreateAsync(new HttpMonitorCheck()
            {
                Id = HttpMonitorCheckId.Create(),
                HttpMonitorId = @event.HttpMonitorId,
                Request = @event.Request,
                Response = @event.Response,
                RequestTiming = @event.RequestTiming,
                ErrorMessage = @event.ErrorMessage
            });
        }
    }
}