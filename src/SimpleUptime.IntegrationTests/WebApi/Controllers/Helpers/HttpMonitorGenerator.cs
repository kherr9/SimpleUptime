﻿using System;
using System.Net.Http;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Helpers
{
    public static class HttpMonitorGenerator
    {
        public static HttpMonitor Generate()
        {
            return new HttpMonitor(
                HttpMonitorId.Create(),
                new HttpRequest()
                {
                    Method = HttpMethod.Get,
                    Url = new Uri($"https://{DateTime.UtcNow.Ticks}.example.com/")
                });
        }
    }
}