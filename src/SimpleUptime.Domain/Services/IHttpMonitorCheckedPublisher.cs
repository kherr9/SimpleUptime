using System.Threading.Tasks;
using SimpleUptime.Domain.Events;

namespace SimpleUptime.Domain.Services
{
    public interface IHttpMonitorCheckedPublisher
    {
        Task PublishAsync(HttpMonitorChecked @event);
    }
}