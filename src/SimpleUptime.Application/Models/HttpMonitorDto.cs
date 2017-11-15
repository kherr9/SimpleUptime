using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Models
{
    public class HttpMonitorDto
    {
        public string Id { get; set; }

        public Uri Url { get; set; }

        public static HttpMonitorDto CreateFrom(HttpMonitor httpMonitor)
        {
            if (httpMonitor == null) throw new ArgumentNullException(nameof(httpMonitor));

            return new HttpMonitorDto
            {
                Id = httpMonitor.Id.Value.ToString(),
                Url = httpMonitor.Url
            };
        }
    }
}