using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Events
{
    public class HttpEndpointChecked
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }

        public HttpRequestTiming RequestTiming { get; set; }

        public string ErrorMessage { get; set; }
    }
}