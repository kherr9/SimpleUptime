using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Repositories
{
    /// <summary>
    /// Repository for <see cref="HttpMonitor"/>
    /// </summary>
    public interface IHttpMonitorRepository
    {
        /// <summary>
        /// Fetch <see cref="HttpMonitor"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<HttpMonitor> GetByIdAsync(HttpMonitorId id);

        /// <summary>
        /// Create or update <see cref="HttpMonitor"/>
        /// </summary>
        /// <param name="httpMonitor"></param>
        /// <returns></returns>
        Task PutAsync(HttpMonitor httpMonitor);

        /// <summary>
        /// Delete <see cref="HttpMonitor"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(HttpMonitorId id);
    }
}
