using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorService
    {
        Task<HttpMonitorDto> CreateHttpMonitorAsync(CreateHttpMonitor command);
    }
}
