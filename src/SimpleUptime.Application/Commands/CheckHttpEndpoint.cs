using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Services
{
    public class CheckHttpEndpoint
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public Uri Url { get; set; }
    }
}