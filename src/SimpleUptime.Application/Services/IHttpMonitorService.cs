using System.Threading.Tasks;
using SimpleUptime.Application.Commands;
using SimpleUptime.Application.Models;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorService
    {
        Task<HttpMonitorDto> GetHttpMonitorByIdAsync(string id);

        Task<HttpMonitorDto> CreateHttpMonitorAsync(CreateHttpMonitor command);

        Task<HttpMonitorDto> UpdateHttpMonitorAsync(UpdateHttpMonitor command);

        Task DeleteHttpMonitorAsync(string id);
    }
}
