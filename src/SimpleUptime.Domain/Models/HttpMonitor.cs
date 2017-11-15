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
        public HttpMonitor(HttpMonitorId id, Uri url)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public HttpMonitorId Id { get; }

        public Uri Url { get; set; }
    }
}