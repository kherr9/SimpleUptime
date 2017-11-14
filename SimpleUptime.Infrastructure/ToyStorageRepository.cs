using System;
using System.Threading.Tasks;
using SimpleUptime.Domain;
using SimpleUptime.Domain.Models;
using ToyStorage;

namespace SimpleUptime.Infrastructure
{
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

        public Task UpdateAsync(HttpMonitor entity)
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
