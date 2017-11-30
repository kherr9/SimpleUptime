using System.Threading.Tasks;
using SimpleUptime.Application.Exceptions;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Application.Services
{
    public class HttpMonitorExecutorService : IHttpMonitorExecutorService
    {
        private readonly IHttpMonitorRepository _repository;
        private readonly IHttpMonitorExecutor _executor;

        public HttpMonitorExecutorService(IHttpMonitorRepository repository, IHttpMonitorExecutor executor)
        {
            _repository = repository;
            _executor = executor;
        }

        public async Task<HttpMonitorCheck> ExecuteAsync(HttpMonitorId httpMonitorId)
        {
            var httpMonitor = await _repository.GetByIdAsync(httpMonitorId);

            if (httpMonitor == null)
            {
                throw new EntityNotFoundException(httpMonitorId);
            }

            var cmd = httpMonitor.CreateCheckHttpEndpoint(HttpMonitorCheckId.Create());

            return await _executor.CheckHttpEndpointAsync(cmd);
        }
    }
}