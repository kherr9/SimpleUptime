using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Repositories
{
    /// <summary>
    /// Repository for <see cref="HttpMonitorCheck"/>
    /// </summary>
    public interface IHttpMonitorCheckRepository
    {
        Task CreateAsync(HttpMonitorCheck httpMonitorCheck);

        Task<HttpMonitorCheck> GetAsync(HttpMonitorCheckId id);

        Task<IEnumerable<HttpMonitorCheck>> GetAsync(HttpMonitorId id);
    }
}