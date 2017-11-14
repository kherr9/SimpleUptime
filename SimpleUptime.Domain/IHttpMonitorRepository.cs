using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain
{
    public interface IHttpMonitorRepository
    {
        Task<HttpMonitor> GetAsync(HttpMonitorId id);

        Task UpdateAsync(HttpMonitor httpMonitor);

        Task DeleteAsync(HttpMonitorId id);
    }
}
