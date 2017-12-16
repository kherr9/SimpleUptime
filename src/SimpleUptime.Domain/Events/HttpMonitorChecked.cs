using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Events
{
    public class HttpMonitorChecked
    {
        public HttpMonitorChecked(HttpMonitorCheck httpMonitorCheck)
        {
            HttpMonitorCheck = httpMonitorCheck ?? throw new ArgumentNullException(nameof(httpMonitorCheck));
        }

        public HttpMonitorCheck HttpMonitorCheck { get; set; }
    }
}