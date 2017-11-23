using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleUptime.Domain.Commands;

namespace SimpleUptime.Domain.Services
{
    public interface ICheckHttpEndpointPublisher
    {
        Task PublishAsync(IEnumerable<CheckHttpEndpoint> commands);
    }
}
