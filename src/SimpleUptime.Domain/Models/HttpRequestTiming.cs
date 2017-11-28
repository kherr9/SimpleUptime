using System;
using System.Diagnostics;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Detail timing information.
    /// </summary>
    [DebuggerDisplay("{StartTime} {EndTime} Duration:{EndTime.Subtract(StartTime).TotalMilliseconds}")]
    public class HttpRequestTiming
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public void SetStartTime()
        {
            if (!StartTime.IsEmpty())
            {
                throw new InvalidOperationException($"Property {nameof(StartTime)} already set.");
            }

            StartTime = DateTime.UtcNow;
        }

        public void SetEndTime()
        {
            if (!EndTime.IsEmpty())
            {
                throw new InvalidOperationException($"Property {nameof(EndTime)} already set.");
            }

            EndTime = DateTime.UtcNow;
        }

        public double TotalMilliseconds => !StartTime.IsEmpty() && !EndTime.IsEmpty() ? EndTime.Subtract(StartTime).TotalMilliseconds : 0d;

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
    }
}