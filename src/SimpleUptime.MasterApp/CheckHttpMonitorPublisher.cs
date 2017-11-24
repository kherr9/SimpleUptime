using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SimpleUptime.MasterApp.Infrastructure;
using SimpleUptime.Application.Services;

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
    }
}