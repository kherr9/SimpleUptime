using System;
using System.Diagnostics;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Detail timing information.
    /// </summary>
    [DebuggerDisplay("{StartTime} {EndTime} {TotalMilliseconds}")]
    public class HttpRequestTiming
    {
        public HttpRequestTiming(DateTime startTime, DateTime endTime)
        {
            ThrowIfInvalid(startTime, nameof(startTime));
            ThrowIfInvalid(endTime, nameof(endTime));

            if (startTime > endTime)
            {
                throw new ArgumentException($"StartTime '{startTime}' can not be greater than EndTime '{endTime}'.");
            }

            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public double TotalMilliseconds => EndTime.Subtract(StartTime).TotalMilliseconds;

        public override bool Equals(object obj)
        {
            return obj is HttpRequestTiming timing &&
                   StartTime == timing.StartTime &&
                   EndTime == timing.EndTime;
        }

        public override int GetHashCode()
        {
            var hashCode = -445957783;
            hashCode = hashCode * -1521134295 + StartTime.GetHashCode();
            hashCode = hashCode * -1521134295 + EndTime.GetHashCode();
            return hashCode;
        }

        private void ThrowIfInvalid(DateTime value, string paramName)
        {
            if (value.IsEmpty() || value == DateTime.MaxValue || value == DateTime.MinValue) throw new ArgumentException($"Invalid value of {value}", paramName);
        }
    }
}