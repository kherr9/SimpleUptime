using System;
using System.ComponentModel;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Events
{
    public class HttpMonitorDown : IEvent
    {
        public HttpMonitorDown(HttpMonitorId httpMonitorId, MonitorStatus previousStatus, DateTime startTime, DateTime created)
        {
            if (!Enum.IsDefined(typeof(MonitorStatus), previousStatus))
                throw new InvalidEnumArgumentException(nameof(previousStatus), (int)previousStatus,
                    typeof(MonitorStatus));

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