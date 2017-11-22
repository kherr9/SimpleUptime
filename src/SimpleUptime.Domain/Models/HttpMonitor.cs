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
}