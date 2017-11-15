using System;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Http check monitor definition.
    /// Configuration of check logic.
    /// </summary>
    public class HttpMonitor
    {
        public HttpMonitorId Id { get; set; }

        public Uri Url { get; set; }
    }
}