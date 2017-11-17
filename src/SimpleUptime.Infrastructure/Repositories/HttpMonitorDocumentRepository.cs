using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;

namespace SimpleUptime.Infrastructure.Repositories
{
    public class HttpMonitorDocumentRepository : IHttpMonitorRepository
    {
        private const string DatabaseId = "SimpleUptimeDb";
        private const string CollectionId = "Entities";
        private readonly IDocumentClient _client;

        public HttpMonitorDocumentRepository(IDocumentClient client)
        {
            _client = client;
        }

        public Task<IEnumerable<HttpMonitor>> GetAsync()
        {
            var uri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var options = new FeedOptions()
            {
                PartitionKey = new PartitionKey(nameof(HttpMonitor))
            };

            var query = _client.CreateDocumentQuery<HttpMonitor>(uri, options)
                .AsDocumentQuery();

            return query.ToEnumerableAsync();
        }

        public Task<HttpMonitor> GetByIdAsync(HttpMonitorId id)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var options = new FeedOptions()
            {
                PartitionKey = new PartitionKey(nameof(HttpMonitor)),
                MaxItemCount = 1
            };

            var query = _client.CreateDocumentQuery<HttpMonitor>(uri, options)
                .Where(x => x.Id == id)
                .AsDocumentQuery();

            return query.FirstOrDefaultAsync();
        }

        public Task PutAsync(HttpMonitor httpMonitor)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(HttpMonitorId id)
        {
            throw new NotImplementedException();
        }
    }
}
