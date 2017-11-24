using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;

namespace SimpleUptime.WorkerApp
{
    public static class Functions
    {
        [FunctionName("HandleCheckHttpMonitorPublisher")]
        public static Task Run(
            [ServiceBusTrigger("master.events", "worker", AccessRights.Listen, Connection = "azureServiceBus")] string data,
            TraceWriter log)
        {
            return Task.CompletedTask;
        }
    }
}
