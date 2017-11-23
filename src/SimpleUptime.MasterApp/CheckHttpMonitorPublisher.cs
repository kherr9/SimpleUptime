using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SimpleUptime.MasterApp
{
    public static class CheckHttpMonitorPublisher
    {
        [FunctionName("CheckHttpMonitorPublisher")]
        public static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
