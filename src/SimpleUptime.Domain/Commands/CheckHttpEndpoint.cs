using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Commands
{
    public class CheckHttpEndpoint
    {
        public CheckHttpEndpoint(
            HttpMonitorCheckId httpMonitorCheckId,
            HttpMonitorId httpMonitorId,
            HttpRequest request)
        {
            HttpMonitorCheckId = httpMonitorCheckId ?? throw new ArgumentNullException(nameof(httpMonitorCheckId));
            HttpMonitorId = httpMonitorId ?? throw new ArgumentNullException(nameof(httpMonitorId));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public HttpMonitorCheckId HttpMonitorCheckId { get; }

        public HttpMonitorId HttpMonitorId { get; }

        public HttpRequest Request { get; }

        public HttpMonitorCheck CreateHttpMonitorCheck(HttpRequestTiming requestTiming, HttpResponse response, string errorMessage)
        {
            return new HttpMonitorCheck(HttpMonitorCheckId, HttpMonitorId, Request, requestTiming, response, errorMessage);
        }
    }
}