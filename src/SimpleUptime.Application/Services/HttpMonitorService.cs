using System;
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

        public async Task<HttpMonitorDto> GetHttpMonitorByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var httpMonitor = await _repository.GetAsync(Guid.Parse(id));

            if (httpMonitor != null)
            {
                return HttpMonitorDto.CreateFrom(httpMonitor);
            }

            return null;
        }

        public async Task<HttpMonitorDto> CreateHttpMonitorAsync(CreateHttpMonitor command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var httpMonitor = new HttpMonitor(HttpMonitorId.Create(), command.Url);

            await _repository.PutAsync(httpMonitor);

            return HttpMonitorDto.CreateFrom(httpMonitor);
        }

        public async Task<HttpMonitorDto> UpdateHttpMonitorAsync(UpdateHttpMonitor command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var httpMonitor = await _repository.GetAsync(Guid.Parse(command.HttpMonitorId));

            if (httpMonitor == null)
            {
                throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {command.HttpMonitorId}");
            }

            httpMonitor.Url = command.Url;

            await _repository.PutAsync(httpMonitor);

            return HttpMonitorDto.CreateFrom(httpMonitor);
        }
    }
}