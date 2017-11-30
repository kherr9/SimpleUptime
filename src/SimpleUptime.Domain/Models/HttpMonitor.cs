using System;
using System.Collections.Generic;
using SimpleUptime.Domain.Commands;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Http check monitor definition.
    /// Configuration of check logic.
    /// </summary>
    public class HttpMonitor
    {
        public HttpMonitor(HttpMonitorId id, HttpRequest request)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            AlertContactIds = new HashSet<AlertContactId>();
        }

        public HttpMonitorId Id { get; }

        public HttpRequest Request { get; private set; }

        public HashSet<AlertContactId> AlertContactIds { get; }

        public void UpdateRequest(HttpRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public CheckHttpEndpoint CreateCheckHttpEndpoint(HttpMonitorCheckId httpMonitorCheckId)
        {
            return new CheckHttpEndpoint(httpMonitorCheckId, Id, Request);
        }
    }
}