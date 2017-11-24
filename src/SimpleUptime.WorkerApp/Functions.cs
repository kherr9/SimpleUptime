using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SimpleUptime.WorkerApp.Infrastructure;

namespace SimpleUptime.WorkerApp
{
    public static class Functions
    {
        [FunctionName("HandleCheckHttpMonitorPublisher")]
        public static async Task Run(
            [ServiceBusTrigger("master.events", "worker", AccessRights.Listen, Connection = "azureServiceBus")] BrokeredMessage data,
            TraceWriter log,
            [Inject] JsonSerializer serializer)
        {
            byte[] content;
            using (var ms = new MemoryStream())
            {
                using (var stream = data.GetBody<Stream>())
                {
                    await stream.CopyToAsync(ms);
                }

                content = ms.ToArray();
            }
            
            var json = System.Text.Encoding.UTF8.GetString(content);

            object obj;
            using (var ms = new MemoryStream(content))
            using (var s = new StreamReader(ms))
            using (var x = new JsonTextReader(s))
            {
                obj = serializer.Deserialize<object>(x);
                ////var typeName = (string)data.Properties["Message-AssemblyQualifiedName"];
                ////var type = Type.GetType(typeName, true);
                ////obj = serializer.Deserialize(x, type);
            }
        }
    }
}
