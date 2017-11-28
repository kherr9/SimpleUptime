using System;
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
            jObject.Add("_type", JValue.CreateString(httpMonitorCheck.GetType().Name));

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
                    new SqlParameter("@type", nameof(HttpMonitorCheck))
                }
            };

            return _client.CreateDocumentQuery<HttpMonitorCheck>(uri, querySpec, options)
                .AsDocumentQuery()
                .FirstOrDefaultAsync();
        }
    }
}
