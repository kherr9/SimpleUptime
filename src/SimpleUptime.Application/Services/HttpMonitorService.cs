using System;
using System.Threading.Tasks;
using SimpleUptime.Application.Commands;
using SimpleUptime.Application.Exceptions;
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

        public Task<HttpMonitor> GetHttpMonitorByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return _repository.GetAsync(Guid.Parse(id));
        }

        public async Task<HttpMonitor> CreateHttpMonitorAsync(CreateHttpMonitor command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var httpMonitor = new HttpMonitor(HttpMonitorId.Create(), command.Url);

            await _repository.PutAsync(httpMonitor);

            return httpMonitor;
        }

        public async Task<HttpMonitor> UpdateHttpMonitorAsync(UpdateHttpMonitor command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var httpMonitor = await _repository.GetAsync(Guid.Parse(command.HttpMonitorId));

            if (httpMonitor == null)
            {
                throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {command.HttpMonitorId}");
            }

            httpMonitor.Url = command.Url;

            await _repository.PutAsync(httpMonitor);

            return httpMonitor;
        }

        public async Task DeleteHttpMonitorAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var httpMonitor = await _repository.GetAsync(Guid.Parse(id));

            if (httpMonitor == null)
            {
                throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {id}");
            }

            await _repository.DeleteAsync(Guid.Parse(id));
        }
    }
}