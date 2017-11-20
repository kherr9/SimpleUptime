using System;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Helpers
{
    public static class EntityGenerator
    {
        public static object GenerateHttpMonitor()
        {
            return new
            {
                Url = new Uri($"https://{DateTime.UtcNow.Ticks}.example.com/")
            };
        }
    }
}