using System;
using System.Collections.Generic;
using System.IO;
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
    public class HttpMonitorCheckDocumentRepository : IHttpMonitorCheckRepository
    {
        private readonly IDocumentClient _client;
        private readonly DatabaseConfigurations _configs;
        private const string DocumentType = nameof(HttpMonitorCheck);

        public HttpMonitorCheckDocumentRepository(IDocumentClient client, DatabaseConfigurations configs)
        {
            _client = client;
            _configs = configs;
        }

        public Task CreateAsync(HttpMonitorCheck httpMonitorCheck)
        {
            if (httpMonitorCheck == null) throw new ArgumentNullException(nameof(httpMonitorCheck));

            var documentcollectionUri = _configs.DocumentCollectionUri;

            var jObject = JObject.FromObject(httpMonitorCheck, JsonSerializer.Create(Constants.JsonSerializerSettings));

            // add type
            jObject.Add("_type", JValue.CreateString(DocumentType));

            var json = jObject.ToString();

            var document = JsonSerializable.LoadFrom<Document>(new MemoryStream(Encoding.UTF8.GetBytes(json)));

            return _client.CreateDocumentAsync(documentcollectionUri, document, null, true);
        }

        public Task<HttpMonitorCheck> GetAsync(HttpMonitorCheckId id)
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

            return _client.CreateDocumentQuery<HttpMonitorCheck>(uri, querySpec, options)
                .AsDocumentQuery()
                .FirstOrDefaultAsync();
        }

        public Task<IEnumerable<HttpMonitorCheck>> GetAsync(HttpMonitorId httpMonitorId)
        {
            if (httpMonitorId == null) throw new ArgumentNullException(nameof(httpMonitorId));

            var uri = _configs.DocumentCollectionUri;

            var querySpec = new SqlQuerySpec
            {
                QueryText = "select * from root r where (r.httpMonitorId = @httpMonitorId and r._type = @type)",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@httpMonitorId", httpMonitorId.ToString()),
                    new SqlParameter("@type", DocumentType)
                }
            };

            return _client.CreateDocumentQuery<HttpMonitorCheck>(uri, querySpec)
                .AsDocumentQuery()
                .ToEnumerableAsync();
        }
    }
}
