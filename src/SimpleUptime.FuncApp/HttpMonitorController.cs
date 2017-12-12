using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
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
        [Inject] IHttpMonitorService service)
        {
            var httpMonitors = await service.GetHttpMonitorsAsync();

            return req.CreateResponse(HttpStatusCode.OK, httpMonitors, MediaTypeHeaderValue.Parse("application/json"));
        }
        
        [FunctionName("HttpMonitorsPost")]
        public static async Task<HttpResponseMessage> PostAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "httpmonitors")]HttpRequestMessage req,
            TraceWriter log,
            [Inject] IHttpMonitorService service)
        {
            var json = await req.Content.ReadAsStringAsync();
            var cmd = JsonConvert.DeserializeObject<CreateHttpMonitor>(json);

            var httpMonitor = await service.CreateHttpMonitorAsync(cmd);

            return req.CreateResponse(HttpStatusCode.OK, httpMonitor, MediaTypeHeaderValue.Parse("application.json"));
        }
    }
}
