using System.Threading.Tasks;
using SimpleUptime.Domain.Events;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorExecutorService
    {
        Task<HttpEndpointChecked> ExecuteAsync(HttpMonitorId httpMonitorId);
    }
}