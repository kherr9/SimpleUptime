using System;

namespace SimpleUptime.Domain.Models
{
    public class HttpMonitorCheck
    {
        public HttpMonitorCheck(
            HttpMonitorCheckId id,
            HttpMonitorId httpMonitorId,
            HttpRequest request,
            HttpRequestTiming requestTiming,
            HttpResponse response = null,
            string errorMessage = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            HttpMonitorId = httpMonitorId ?? throw new ArgumentNullException(nameof(httpMonitorId));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            RequestTiming = requestTiming ?? throw new ArgumentNullException(nameof(requestTiming));
            Response = response;
            ErrorMessage = errorMessage;
        }

        public HttpMonitorCheckId Id { get; }

        public HttpMonitorId HttpMonitorId { get; }

        public HttpRequest Request { get; }

        public HttpRequestTiming RequestTiming { get; }

        public HttpResponse Response { get; }
        
        public string ErrorMessage { get; }
    }
}