using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using ToyStorage;

namespace SimpleUptime.Infrastructure.Repositories
{
    /// <inheritdoc />
    /// <summary>
    /// Repository for <see cref="HttpMonitor"/> using <see cref="IDocumentCollection"/>
    /// </summary>
    public class HttpMonitorRepository : IHttpMonitorRepository
    {
        private readonly IDocumentCollection _documentCollection;
        private static readonly string DocumentId = nameof(HttpMonitor).ToLowerInvariant();

        public HttpMonitorRepository(IDocumentCollection documentCollection)
        {
            _documentCollection = documentCollection;
        }

        public async Task<IEnumerable<HttpMonitor>> GetAsync()
        {
            var dict = await InnerGetAsync();

            return dict.Values;
        }

        public async Task<HttpMonitor> GetByIdAsync(HttpMonitorId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var dict = await InnerGetAsync();

            return dict.Values.FirstOrDefault(x => x.Id == id);
        }

        public async Task PutAsync(HttpMonitor httpMonitor)
        {
            if (httpMonitor == null) throw new ArgumentNullException(nameof(httpMonitor));

            var dict = await InnerGetAsync();

            dict[httpMonitor.Id] = httpMonitor;

            await InnerWriteAsync(dict);
        }

        public async Task DeleteAsync(HttpMonitorId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var dict = await InnerGetAsync();

            if (dict.Remove(id))
            {
                await InnerWriteAsync(dict);
            }
        }

        private async Task<Dictionary<HttpMonitorId, HttpMonitor>> InnerGetAsync()
        {
            return ((await _documentCollection.GetAsync<KeyValuePair<HttpMonitorId, HttpMonitor>[]>(DocumentId)) ?? new KeyValuePair<HttpMonitorId, HttpMonitor>[0]).ToDictionary(x => x.Key, x => x.Value);
        }

        private Task InnerWriteAsync(Dictionary<HttpMonitorId, HttpMonitor> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            return _documentCollection.PutAsync(dict.ToArray(), DocumentId);
        }
    }
}
