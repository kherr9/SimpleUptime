using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Commands
{
    public class UpdateHttpMonitor
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public HttpRequest Request { get; set; }
    }
}