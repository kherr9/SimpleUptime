using System;
using System.Collections.Generic;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Helpers
{
    public static class HttpMonitorIdHelper
    {
        public static IEnumerable<object[]> InvalidHttpMonitorIds()
        {
            yield return new object[] { "foo" };
            yield return new object[] { 1 };
            yield return new object[] { 0 };
            yield return new object[] { -1 };
            yield return new object[] { int.MaxValue };
            yield return new object[] { long.MaxValue };
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { Guid.Empty.ToString() };
        }
    }
}
