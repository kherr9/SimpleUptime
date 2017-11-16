using System.Threading.Tasks;
using SimpleUptime.Application.Commands;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorService
    {
        Task<HttpMonitor> GetHttpMonitorByIdAsync(string id);

        Task<HttpMonitor> CreateHttpMonitorAsync(CreateHttpMonitor command);

        Task<HttpMonitor> UpdateHttpMonitorAsync(UpdateHttpMonitor command);

        Task DeleteHttpMonitorAsync(string id);
    }
}
