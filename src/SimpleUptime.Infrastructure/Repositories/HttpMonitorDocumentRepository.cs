using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using Constants = SimpleUptime.Infrastructure.Services.Constants;

namespace SimpleUptime.Infrastructure.Repositories
{
    public class HttpMonitorDocumentRepository : IHttpMonitorRepository
    {
        private readonly IDocumentClient _client;
        private readonly DatabaseConfigurations _configs;
        private const string DocumentType = nameof(HttpMonitor);

        public HttpMonitorDocumentRepository(IDocumentClient client, DatabaseConfigurations configs)
        {
            _client = client;
            _configs = configs;
        }

        public Task<IEnumerable<HttpMonitor>> GetAsync()
        {
            var uri = _configs.DocumentCollectionUri;

            var querySpec = new SqlQuerySpec
            {
                QueryText = "select * from root r where (r._type = @type)",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@type", DocumentType)
                }
            };

            return _client.CreateDocumentQuery<HttpMonitor>(uri, querySpec)
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
                QueryText = "select * from root r where (r.id = @id and r._type = @type)",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@id", id.ToString()),
                    new SqlParameter("@type", DocumentType)
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

            var jObject = JObject.FromObject(httpMonitor, JsonSerializer.Create(Constants.JsonSerializerSettings));

            // add type
            jObject.Add("_type", JValue.CreateString(DocumentType));

            var document = JsonSerializable.LoadFrom<Document>(new MemoryStream(Encoding.UTF8.GetBytes(jObject.ToString())));

            return _client.UpsertDocumentAsync(documentCollectionUri, document, null, true);
        }

        public async Task DeleteAsync(HttpMonitorId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var uri = _configs.DocumentUri(id.ToString());

            try
            {
                // todo: doesn't check type
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
