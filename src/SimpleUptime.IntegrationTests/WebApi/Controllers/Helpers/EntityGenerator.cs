using System;
using System.Net.Http;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Helpers
{
    public static class EntityGenerator
    {
        public static object GenerateHttpMonitor()
        {
            return new
            {
                Request = new
                {
                    Url = new Uri($"https://{DateTime.UtcNow.Ticks}.example.com/"),
                    Method = "GET"
                }
            };
        }
    }

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