using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Events
{
    public class HttpMonitorUp : IEvent
    {
        public HttpMonitorUp(HttpMonitorId httpMonitorId, MonitorStatus previousStatus, DateTime startTime, DateTime created)
        {
            HttpMonitorId = httpMonitorId ?? throw new ArgumentNullException(nameof(httpMonitorId));
            PreviousStatus = previousStatus;
            StartTime = startTime;
            Created = created;
        }

        public HttpMonitorId HttpMonitorId { get; }

        public MonitorStatus PreviousStatus { get; }

        public DateTime StartTime { get; }

        public DateTime Created { get; }
    }
}