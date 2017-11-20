using System.Threading.Tasks;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Events;

namespace SimpleUptime.Domain.Services
{
    public interface IHttpMonitorExecutor
    {
        Task<HttpEndpointChecked> CheckHttpEndpointAsync(CheckHttpEndpoint command);
    }
}