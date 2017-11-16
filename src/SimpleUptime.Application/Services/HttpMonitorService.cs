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

        private static readonly Task<HttpMonitor> NullHttpMonitorTask = Task.FromResult<HttpMonitor>(null);

        public HttpMonitorService(IHttpMonitorRepository repository)
        {
            _repository = repository;
        }

        public Task<HttpMonitor> GetHttpMonitorByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (Guid.TryParse(id, out var guid))
            {
                return _repository.GetAsync(guid);
            }

            return NullHttpMonitorTask;
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

            if (Guid.TryParse(command.HttpMonitorId, out var guid))
            {
                var httpMonitor = await _repository.GetAsync(guid);

                if (httpMonitor == null)
                {
                    throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {command.HttpMonitorId}");
                }

                httpMonitor.Url = command.Url;

                await _repository.PutAsync(httpMonitor);

                return httpMonitor;
            }

            throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {command.HttpMonitorId}");
        }

        public async Task DeleteHttpMonitorAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (Guid.TryParse(id, out var guid))
            {
                var httpMonitor = await _repository.GetAsync(guid);

                if (httpMonitor == null)
                {
                    throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {id}");
                }

                await _repository.DeleteAsync(guid);
            }
            else
            {
                throw new EntityNotFoundException($"Unknown {nameof(HttpMonitor)} with id {id}");
            }
        }
    }
}