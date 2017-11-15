using System.Threading.Tasks;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorService
    {
        Task<HttpMonitorDto> GetHttpMonitorByIdAsync(string id);

        Task<HttpMonitorDto> CreateHttpMonitorAsync(CreateHttpMonitor command);

        Task<HttpMonitorDto> UpdateHttpMonitorAsync(UpdateHttpMonitor command);
    }
}
