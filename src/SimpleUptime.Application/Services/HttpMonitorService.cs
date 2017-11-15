using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;

namespace SimpleUptime.Application.Services
{
    public class HttpMonitorService : IHttpMonitorService
    {
        private readonly IHttpMonitorRepository _repository;

        public HttpMonitorService(IHttpMonitorRepository repository)
        {
            _repository = repository;
        }

        public async Task<HttpMonitorDto> CreateHttpMonitorAsync(CreateHttpMonitor command)
        {
            var httpMonitor = new HttpMonitor(HttpMonitorId.Create(), command.Url);

            await _repository.PutAsync(httpMonitor);

            return new HttpMonitorDto()
            {
                Id = httpMonitor.Id.Value.ToString(),
                Url = httpMonitor.Url.ToString()
            };
        }
    }
}