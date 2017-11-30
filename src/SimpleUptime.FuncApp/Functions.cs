using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Events;
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
            [Inject] IHttpMonitorCheckRepository repository,
            [Inject] IHttpMonitorCheckedPublisher publisher)
        {
            var check = JsonConvert.DeserializeObject<CheckHttpEndpoint>(json, Constants.JsonSerializerSettings);

            var httpMonitorCheck = await executor.CheckHttpEndpointAsync(check);

            await repository.CreateAsync(httpMonitorCheck);

            var @event = httpMonitorCheck.CreateHttpMonitorChecked();

            await publisher.PublishAsync(@event);
        }

        [FunctionName("HttpMonitorHandlesHttpMonitorChecked")]
        public static async Task HttpMonitorHandlesHttpMonitorChecked(
            [QueueTrigger("events-httpmonitorchecked-httpmonitor")] string json,
            TraceWriter log,
            [Inject] IHttpMonitorRepository repository)
        {
            var @event = JsonConvert.DeserializeObject<HttpMonitorChecked>(json, Constants.JsonSerializerSettings);

            var monitor = await repository.GetByIdAsync(@event.HttpMonitorCheck.HttpMonitorId);

            monitor?.Handle(@event);

            await repository.PutAsync(monitor);
        }
    }
};