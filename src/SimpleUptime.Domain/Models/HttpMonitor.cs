using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public HttpMonitor(HttpMonitorId id, HttpRequest request, IEnumerable<HttpMonitorCheck> recentHttpMonitorChecks = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            AlertContactIds = new HashSet<AlertContactId>();

            RecentHttpMonitorChecks = recentHttpMonitorChecks != null ?
                recentHttpMonitorChecks.ToList().AsReadOnly() :
                new List<HttpMonitorCheck>().AsReadOnly();
        }

        public HttpMonitorId Id { get; }

        public HttpRequest Request { get; private set; }

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

            const int maxCount = 10;

            // check if check already exists
            if (RecentHttpMonitorChecks.All(x => x.Id != @event.HttpMonitorCheck.Id))
            {
                var set = new List<HttpMonitorCheck>(RecentHttpMonitorChecks)
                {
                    @event.HttpMonitorCheck
                };

                RecentHttpMonitorChecks = set
                    .OrderByDescending(x => x.RequestTiming.StartTime)
                    .Take(maxCount)
                    .ToList()
                    .AsReadOnly();
            }
        }
    }
}