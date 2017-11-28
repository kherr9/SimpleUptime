using System;

namespace SimpleUptime.Domain.Models
{
    public class HttpMonitorCheck
    {
        ////public HttpMonitorCheckId Id { get; set; }
        public Guid Id { get; set; }

        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }

        public HttpRequestTiming RequestTiming { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}