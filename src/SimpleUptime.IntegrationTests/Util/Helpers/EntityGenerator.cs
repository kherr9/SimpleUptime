using System;

namespace SimpleUptime.IntegrationTests.Util.Helpers
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
}