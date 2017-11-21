using System;
using Xunit;

namespace SimpleUptime.IntegrationTests
{
    public static class AssertDateTime
    {
        public static void Equal(DateTime expected, DateTime actual, TimeSpan tolerance)
        {
            var dif = Math.Abs(expected.Subtract(actual).TotalMilliseconds);

            Assert.True(dif < tolerance.TotalMilliseconds, $"Expected: {expected}, Actual: {actual}, DifferenceMilliseconds: {dif}");
        }
    }
}