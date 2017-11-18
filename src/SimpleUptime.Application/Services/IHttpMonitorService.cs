using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleUptime.Application.Commands;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorService
    {
        Task<IEnumerable<HttpMonitor>> GetHttpMonitorsAsync();

        Task<HttpMonitor> GetHttpMonitorByIdAsync(HttpMonitorId httpMonitorId);

        Task<HttpMonitor> CreateHttpMonitorAsync(CreateHttpMonitor command);

        Task<HttpMonitor> UpdateHttpMonitorAsync(UpdateHttpMonitor command);

        Task DeleteHttpMonitorAsync(HttpMonitorId httpMonitorId);
    }
}
