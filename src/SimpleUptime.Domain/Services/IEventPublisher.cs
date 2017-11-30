using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleUptime.Domain.Events;

namespace SimpleUptime.Domain.Services
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEnumerable<IEvent> events);
    }
}
