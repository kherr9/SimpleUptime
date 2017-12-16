using System.Threading.Tasks;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Services
{
    public interface IHttpMonitorExecutor
    {
        Task<HttpMonitorCheck> CheckHttpEndpointAsync(CheckHttpEndpoint command);
    }
}