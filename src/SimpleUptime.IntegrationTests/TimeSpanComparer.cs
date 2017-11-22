using System;

namespace SimpleUptime.IntegrationTests
{
    public static class TimeSpanComparer
    {
        public static readonly TimeSpan DefaultTolerance = TimeSpan.FromMilliseconds(100);
    }
}