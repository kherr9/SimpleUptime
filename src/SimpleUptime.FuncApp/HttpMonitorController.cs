using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SimpleUptime.Application.Commands;
using SimpleUptime.Application.Services;
using SimpleUptime.FuncApp.Infrastructure;

namespace SimpleUptime.FuncApp
{
    public static class HttpMonitorController
    {
        [FunctionName("HttpMonitorsGet")]
        public static async Task<HttpResponseMessage> GetAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "httpmonitors")]HttpRequestMessage req,
        TraceWriter log,
        [Inject] IHttpMonitorService service,
        [Inject] JsonMediaTypeFormatter formatter)
        {
            var httpMonitors = await service.GetHttpMonitorsAsync();

            return req.CreateResponse(HttpStatusCode.OK, httpMonitors, formatter);
        }

        [FunctionName("HttpMonitorsPost")]
        public static async Task<HttpResponseMessage> PostAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "httpmonitors")]HttpRequestMessage req,
            TraceWriter log,
            [Inject] IHttpMonitorService service,
            [Inject] JsonMediaTypeFormatter formatter)
        {
            var cmd = await req.Content.ReadAsAsync<CreateHttpMonitor>(new[] { formatter });

            var httpMonitor = await service.CreateHttpMonitorAsync(cmd);

            return req.CreateResponse(HttpStatusCode.OK, httpMonitor, formatter);
        }
    }
}
