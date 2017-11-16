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

        public Task<HttpMonitor> GetHttpMonitorByIdAsync(HttpMonitorId httpMonitorId)
        {
            if (httpMonitorId == null) throw new ArgumentNullException(nameof(httpMonitorId));

            return _repository.GetAsync(httpMonitorId);
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

            var httpMonitor = await _repository.GetAsync(command.HttpMonitorId);

            if (httpMonitor == null)
            {
                throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {command.HttpMonitorId}");
            }

            httpMonitor.Update(command.Url);

            await _repository.PutAsync(httpMonitor);

            return httpMonitor;
        }

        public async Task DeleteHttpMonitorAsync(HttpMonitorId httpMonitorId)
        {
            if (httpMonitorId == null) throw new ArgumentNullException(nameof(httpMonitorId));

            var httpMonitor = await _repository.GetAsync(httpMonitorId);

            if (httpMonitor == null)
            {
                throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {httpMonitorId}");
            }

            await _repository.DeleteAsync(httpMonitorId);
        }
    }
}