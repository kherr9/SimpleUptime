using System;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Http check monitor definition.
    /// Configuration of check logic.
    /// </summary>
    public class HttpMonitor
    {
        public HttpMonitor()
        {
        }

        public HttpMonitor(HttpMonitorId id, HttpRequest request)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public HttpMonitorId Id { get; set; }

        public HttpRequest Request { get; set; }

        public void UpdateRequest(HttpRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }

    public class HttpMonitorCheckId
    {

    }

    public class HttpMonitorCheck
    {
        public HttpMonitorCheckId Id { get; set; }

        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }

        public HttpRequestTiming RequestTiming { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}