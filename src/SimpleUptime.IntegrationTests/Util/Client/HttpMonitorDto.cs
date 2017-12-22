using System;
using System.Collections.Generic;
using SimpleUptime.Domain.Models;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.IntegrationTests.Util.Client
{
    public class HttpMonitorDto
    {
        public string Id { get; set; }

        public HttpRequestDto Request { get; set; }

        public static HttpMonitorDto CreateFrom(HttpMonitor httpMonitor)
        {
            if (httpMonitor == null) throw new ArgumentNullException(nameof(httpMonitor));

            return new HttpMonitorDto
            {
                Id = httpMonitor.Id.Value.ToString(),
                Request = new HttpRequestDto()
                {
                    Method = httpMonitor.Request.Method.Method,
                    Url = httpMonitor.Request.Url
                }
            };
        }

        public override bool Equals(object obj)
        {
            return obj is HttpMonitorDto dto &&
                   Id == dto.Id &&
                   EqualityComparer<HttpRequestDto>.Default.Equals(Request, dto.Request);
        }

        public override int GetHashCode()
        {
            var hashCode = -531886;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<HttpRequestDto>.Default.GetHashCode(Request);
            return hashCode;
        }
    }
}