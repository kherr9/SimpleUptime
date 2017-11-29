using System.Linq;
using System.Threading.Tasks;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Application.Services
{
    public interface ICheckHttpMonitorPublisherService
    {
        Task PublishAsync();
    }

    public class CheckHttpMonitorPublisherService : ICheckHttpMonitorPublisherService
    {
        private readonly IHttpMonitorRepository _repository;
        private readonly ICheckHttpEndpointPublisher _publisher;

        public CheckHttpMonitorPublisherService(IHttpMonitorRepository repository, ICheckHttpEndpointPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task PublishAsync()
        {
            var httpMonitors = await _repository.GetAsync();

            var commands = httpMonitors.Select(x => new CheckHttpEndpoint()
            {
                HttpMonitorId = x.Id,
                Request = x.Request
            });

            await _publisher.PublishAsync(commands);
        }
    }
}
