using System;

namespace SimpleUptime.Domain.Models
{
    internal static class DateTimeExtension
    {
        public static bool IsEmpty(this DateTime value)
        {
            return value == default(DateTime);
        }
    }
}