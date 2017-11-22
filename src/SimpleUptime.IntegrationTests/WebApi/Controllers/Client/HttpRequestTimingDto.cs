using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Client
{
    public class HttpRequestTimingDto
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HttpRequestTimingDto dto &&
                   StartTime == dto.StartTime &&
                   EndTime == dto.EndTime;
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