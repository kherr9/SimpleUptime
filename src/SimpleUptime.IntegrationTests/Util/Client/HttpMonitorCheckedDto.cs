using System.Collections.Generic;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.IntegrationTests.Util.Client
{
    public class HttpMonitorCheckedDto
    {
        public string HttpMonitorId { get; set; }

        public HttpRequestDto Request { get; set; }

        public HttpResponseDto Response { get; set; }

        public HttpRequestTimingDto RequestTiming { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HttpMonitorCheckedDto dto &&
                   HttpMonitorId == dto.HttpMonitorId &&
                   EqualityComparer<HttpRequestDto>.Default.Equals(Request, dto.Request) &&
                   EqualityComparer<HttpResponseDto>.Default.Equals(Response, dto.Response) &&
                   EqualityComparer<HttpRequestTimingDto>.Default.Equals(RequestTiming, dto.RequestTiming);
        }

        public override int GetHashCode()
        {
            var hashCode = 949617584;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HttpMonitorId);
            hashCode = hashCode * -1521134295 + EqualityComparer<HttpRequestDto>.Default.GetHashCode(Request);
            hashCode = hashCode * -1521134295 + EqualityComparer<HttpResponseDto>.Default.GetHashCode(Response);
            hashCode = hashCode * -1521134295 + EqualityComparer<HttpRequestTimingDto>.Default.GetHashCode(RequestTiming);
            return hashCode;
        }
    }
}