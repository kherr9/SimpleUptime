using System;
using System.Net;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Result of performaning a <see cref="HttpMonitor"/>
    /// </summary>
    public class HttpMonitorCheckResult
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public DateTime Created { get; set; }
    }
}