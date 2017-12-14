using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SimpleUptime.Application.Commands;
using SimpleUptime.Application.Exceptions;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Models;
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

        [FunctionName("HttpMonitorsGetById")]
        public static async Task<HttpResponseMessage> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "httpmonitors/{httpMonitorId}")]HttpRequestMessage req,
            string httpMonitorId,
            TraceWriter log,
            [Inject] IHttpMonitorService service,
            [Inject] JsonMediaTypeFormatter formatter)
        {
            if (!HttpMonitorId.TryParse(httpMonitorId, out var id)) return req.CreateResponse(HttpStatusCode.NotFound);

            var httpMonitor = await service.GetHttpMonitorByIdAsync(id);

            if (httpMonitor != null)
            {
                return req.CreateResponse(HttpStatusCode.OK, httpMonitor, formatter);
            }

            return req.CreateResponse(HttpStatusCode.NotFound);
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

        [FunctionName("HttpMonitorsPut")]
        public static async Task<HttpResponseMessage> PutAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "httpmonitors/{httpMonitorId}")]HttpRequestMessage req,
            string httpMonitorId,
            TraceWriter log,
            [Inject] IHttpMonitorService service,
            [Inject] JsonMediaTypeFormatter formatter)
        {
            if (!HttpMonitorId.TryParse(httpMonitorId, out var id)) return req.CreateResponse(HttpStatusCode.NotFound);

            try
            {
                var cmd = await req.Content.ReadAsAsync<UpdateHttpMonitor>(new[] { formatter });

                cmd.HttpMonitorId = id;

                var httpMonitor = await service.UpdateHttpMonitorAsync(cmd);

                return req.CreateResponse(HttpStatusCode.OK, httpMonitor, formatter);
            }
            catch (EntityNotFoundException)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
        }

        [FunctionName("HttpMonitorsDelete")]
        public static async Task<HttpResponseMessage> DeleteAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "httpmonitors/{httpMonitorId}")]HttpRequestMessage req,
            string httpMonitorId,
            TraceWriter log,
            [Inject] IHttpMonitorService service,
            [Inject] JsonMediaTypeFormatter formatter)
        {
            if (!HttpMonitorId.TryParse(httpMonitorId, out var id)) return req.CreateResponse(HttpStatusCode.NotFound);

            try
            {
                await service.DeleteHttpMonitorAsync(id);

                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (EntityNotFoundException)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
