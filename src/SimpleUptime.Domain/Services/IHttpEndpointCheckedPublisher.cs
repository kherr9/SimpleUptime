using System.Threading.Tasks;
using SimpleUptime.Domain.Events;

namespace SimpleUptime.Domain.Services
{
    public interface IHttpEndpointCheckedPublisher
    {
        Task PublishAsync(HttpEndpointChecked @event);
    }
}