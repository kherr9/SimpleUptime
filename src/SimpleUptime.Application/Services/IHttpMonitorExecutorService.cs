using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorExecutorService
    {
        Task<HttpMonitorCheck> ExecuteAsync(HttpMonitorId httpMonitorId);
    }
}