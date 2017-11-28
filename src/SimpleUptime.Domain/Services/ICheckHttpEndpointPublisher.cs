using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Events;

namespace SimpleUptime.Domain.Services
{
    public interface ICheckHttpEndpointPublisher
    {
        Task PublishAsync(IEnumerable<CheckHttpEndpoint> commands);
    }

    public interface IHttpEndpointCheckedPublisher
    {
        Task PublishAsync(HttpEndpointChecked @event);
    }
}
