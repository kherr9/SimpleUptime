using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Domain.Events
{
    public class HttpEndpointChecked
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }

        public HttpRequestTime RequestTime { get; set; }
    }
}