using System;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Detail timing information.
    /// </summary>
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
    }
}