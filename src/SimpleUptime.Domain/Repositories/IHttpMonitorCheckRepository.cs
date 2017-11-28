using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Repositories
{
    /// <summary>
    /// Repository for <see cref="HttpMonitorCheck"/>
    /// </summary>
    public interface IHttpMonitorCheckRepository
    {
        Task PutAsync(HttpMonitorCheck httpMonitorCheck);
    }
}