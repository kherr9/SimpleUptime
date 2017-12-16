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
    public class AlertContactDocumentRepository : IAlertContactRepository
    {
        private readonly IDocumentClient _client;
        private readonly DatabaseConfigurations _configs;
        private const string DocumentType = "AlertContact";

        public AlertContactDocumentRepository(IDocumentClient client, DatabaseConfigurations configs)
        {
            _client = client;
            _configs = configs;
        }

        public Task CreateAsync(IAlertContact alertContact)
        {
            if (alertContact == null) throw new ArgumentNullException(nameof(alertContact));

            var documentcollectionUri = _configs.DocumentCollectionUri;

            var jObject = JObject.FromObject(alertContact, JsonSerializer.Create(Constants.JsonSerializerSettings));

            // add type
            jObject.Add("_type", JValue.CreateString(DocumentType));

            var json = jObject.ToString();

            var document = JsonSerializable.LoadFrom<Document>(new MemoryStream(Encoding.UTF8.GetBytes(json)));

            return _client.CreateDocumentAsync(documentcollectionUri, document, null, true);
        }

        public Task<IEnumerable<IAlertContact>> GetAsync()
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

            return _client.CreateDocumentQuery<IAlertContact>(uri, querySpec)
                .AsDocumentQuery()
                .ToEnumerableAsync();
        }

        public Task<T> GetAsync<T>(AlertContactId id) where T : IAlertContact
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

            return _client.CreateDocumentQuery<T>(uri, querySpec, options)
                .AsDocumentQuery()
                .FirstOrDefaultAsync();
        }

        public Task<IAlertContact> GetAsync(AlertContactId id)
        {
            return GetAsync<IAlertContact>(id);
        }
    }
}
