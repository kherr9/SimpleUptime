using System;
using SimpleUptime.Domain.Models;
using System.Collections.Generic;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers
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

        public override bool Equals(object obj)
        {
            return obj is HttpMonitorDto dto &&
                   Id == dto.Id &&
                   EqualityComparer<Uri>.Default.Equals(Url, dto.Url);
        }

        public override int GetHashCode()
        {
            var hashCode = 315393214;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<Uri>.Default.GetHashCode(Url);
            return hashCode;
        }
    }
}