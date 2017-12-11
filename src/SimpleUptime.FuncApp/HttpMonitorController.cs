using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SimpleUptime.Application.Services;
using SimpleUptime.FuncApp.Infrastructure;

namespace SimpleUptime.FuncApp
{
    public static class HttpMonitorController
    {
        [FunctionName("HttpMonitorsGet")]
        public static HttpResponseMessage GetAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "httpmonitors")]HttpRequestMessage req,
            TraceWriter log)
        {
            return req.CreateResponse(HttpStatusCode.OK, "Success me");
        }
    }
}
