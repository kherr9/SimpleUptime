using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
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

        public Task PutAsync(HttpMonitorCheck httpMonitorCheck)
        {
            if (httpMonitorCheck == null) throw new ArgumentNullException(nameof(httpMonitorCheck));

            var documentcollectionUri = _configs.DocumentCollectionUri;

            var jObject = JObject.FromObject(httpMonitorCheck, JsonSerializer.Create(Constants.JsonSerializerSettings));

            // add type
            jObject.Add("_type", JValue.CreateString(httpMonitorCheck.GetType().Name));

            var json = jObject.ToString();

            var document = JsonSerializable.LoadFrom<Document>(new MemoryStream(Encoding.UTF8.GetBytes(json)));

            return _client.UpsertDocumentAsync(documentcollectionUri, document, null, true);
        }
    }
}
