using System;
using System.Collections.Generic;
using System.Net;
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
        private readonly IDocumentClient _client;
        private readonly DatabaseConfigurations _configs;

        public HttpMonitorDocumentRepository(IDocumentClient client, DatabaseConfigurations configs)
        {
            _client = client;
            _configs = configs;
        }

        public Task<IEnumerable<HttpMonitor>> GetAsync()
        {
            var uri = _configs.DocumentCollectionUri;

            return _client.CreateDocumentQuery<HttpMonitor>(uri)
                .AsDocumentQuery()
                .ToEnumerableAsync();
        }

        public Task<HttpMonitor> GetByIdAsync(HttpMonitorId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var uri = _configs.DocumentCollectionUri;
            var options = new FeedOptions()
            {
                MaxItemCount = 1
            };

            var querySpec = new SqlQuerySpec
            {
                QueryText = "select * from root r where (r.id = @id)",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@id", id.ToString())
                }
            };

            return _client.CreateDocumentQuery<HttpMonitor>(uri, querySpec, options)
                .AsDocumentQuery()
                .FirstOrDefaultAsync();
        }

        public Task PutAsync(HttpMonitor httpMonitor)
        {
            if (httpMonitor == null) throw new ArgumentNullException(nameof(httpMonitor));

            var documentCollectionUri = _configs.DocumentCollectionUri;

            return _client.UpsertDocumentAsync(documentCollectionUri, httpMonitor, null, true);
        }

        public async Task DeleteAsync(HttpMonitorId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var uri = _configs.DocumentUri(id.ToString());

            try
            {
                await _client.DeleteDocumentAsync(uri);
            }
            catch (DocumentClientException ex)
            {
                // don't throw if document not found
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }
    }
}
