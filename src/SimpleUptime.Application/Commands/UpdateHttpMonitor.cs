using System;

namespace SimpleUptime.Application.Commands
{
    public class UpdateHttpMonitor
    {
        public string HttpMonitorId { get; set; }

        public Uri Url { get; set; }
    }
}