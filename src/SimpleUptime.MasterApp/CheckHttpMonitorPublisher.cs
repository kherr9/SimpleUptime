using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.MasterApp.Infrastructure;

namespace SimpleUptime.MasterApp
{
    public static class CheckHttpMonitorPublisher
    {
        [FunctionName("CheckHttpMonitorPublisher")]
        public static Task Run(
            [TimerTrigger("0 * * * * *", RunOnStartup = true)]TimerInfo myTimer,
            TraceWriter log,
            [Inject]IHttpMonitorRepository repo)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}
