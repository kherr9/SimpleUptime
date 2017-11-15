using System;

namespace SimpleUptime.Application.Services
{
    public class UpdateHttpMonitor
    {
        public string HttpMonitorId { get; set; }

        public Uri Url { get; set; }
    }
}