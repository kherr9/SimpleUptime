using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Events;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Http check monitor definition.
    /// Configuration of check logic.
    /// </summary>
    public class HttpMonitor
    {
        public HttpMonitor(
            HttpMonitorId id,
            HttpRequest request,
            MonitorStatus status = MonitorStatus.Unknown,
            IEnumerable<HttpMonitorCheck> recentHttpMonitorChecks = null)
        {
            if (!Enum.IsDefined(typeof(MonitorStatus), status))
                throw new InvalidEnumArgumentException(nameof(status), (int)status, typeof(MonitorStatus));

            Id = id ?? throw new ArgumentNullException(nameof(id));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Status = status;
            AlertContactIds = new HashSet<AlertContactId>();

            RecentHttpMonitorChecks = recentHttpMonitorChecks != null ?
                recentHttpMonitorChecks.ToList().AsReadOnly() :
                new List<HttpMonitorCheck>().AsReadOnly();
        }

        public HttpMonitorId Id { get; }

        public HttpRequest Request { get; private set; }

        public MonitorStatus Status { get; private set; }

        public HashSet<AlertContactId> AlertContactIds { get; }

        public ReadOnlyCollection<HttpMonitorCheck> RecentHttpMonitorChecks { get; private set; }

        public void UpdateRequest(HttpRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public CheckHttpEndpoint CreateCheckHttpEndpoint(HttpMonitorCheckId httpMonitorCheckId)
        {
            return new CheckHttpEndpoint(httpMonitorCheckId, Id, Request);
        }

        public void Handle(HttpMonitorChecked @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            if (@event.HttpMonitorCheck.HttpMonitorId != Id)
            {
                throw new InvalidOperationException($"Conflicting ids. Expected {Id}, but got {@event.HttpMonitorCheck.HttpMonitorId}.");
            }

            const int maxCount = 10;

            // check if check already exists
            if (RecentHttpMonitorChecks.All(x => x.Id != @event.HttpMonitorCheck.Id))
            {
                var set = new List<HttpMonitorCheck>(RecentHttpMonitorChecks)
                {
                    @event.HttpMonitorCheck
                };

                set = set
                    .OrderByDescending(x => x.RequestTiming.StartTime)
                    .Take(maxCount)
                    .ToList();

                // did we add it...
                if (set.Contains(@event.HttpMonitorCheck))
                {
                    // todo: act on state change
                    Status = CalculateMonitorStatus(set);

                    RecentHttpMonitorChecks = set.AsReadOnly();
                }
            }
        }

        private MonitorStatus CalculateMonitorStatus(IEnumerable<HttpMonitorCheck> httpMonitorChecks)
        {
            var httpMonitorCheck = httpMonitorChecks
                .OrderByDescending(x => x.RequestTiming.StartTime)
                .FirstOrDefault();

            if (httpMonitorCheck == null)
            {
                return MonitorStatus.Unknown;
            }

            if (httpMonitorCheck.ErrorMessage != null)
            {
                return MonitorStatus.Down;
            }

            var httpStatusCode = (int)httpMonitorCheck.Response.StatusCode;

            if (httpStatusCode >= 200 && httpStatusCode < 300)
            {
                return MonitorStatus.Up;
            }

            return MonitorStatus.Down;
        }
    }
}