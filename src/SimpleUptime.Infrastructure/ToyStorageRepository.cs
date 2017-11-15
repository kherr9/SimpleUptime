using System;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using ToyStorage;

namespace SimpleUptime.Infrastructure
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

        public Task PutAsync(HttpMonitor entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var documentId = ToDocumentId(entity.Id);

            return _documentCollection.PutAsync(entity, documentId);
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
