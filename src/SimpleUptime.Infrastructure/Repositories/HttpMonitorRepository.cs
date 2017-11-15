using System;
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

        public HttpMonitorRepository(IDocumentCollection documentCollection)
        {
            _documentCollection = documentCollection;
        }

        public Task<HttpMonitor> GetAsync(HttpMonitorId id)
        {
            var documentId = ToDocumentId(id);

            return _documentCollection.GetAsync<HttpMonitor>(documentId);
        }

        public Task PutAsync(HttpMonitor httpMonitor)
        {
            if (httpMonitor == null) throw new ArgumentNullException(nameof(httpMonitor));

            var documentId = ToDocumentId(httpMonitor.Id);

            return _documentCollection.PutAsync(httpMonitor, documentId);
        }

        public Task DeleteAsync(HttpMonitorId id)
        {
            var documentId = ToDocumentId(id);

            return _documentCollection.DeleteAsync(documentId);
        }

        private static string ToDocumentId(HttpMonitorId id)
        {
            return id.Value.ToString();
        }
    }
}
