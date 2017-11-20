using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Commands
{
    public class CheckHttpEndpoint
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpRequest Request { get; set; }
    }
}