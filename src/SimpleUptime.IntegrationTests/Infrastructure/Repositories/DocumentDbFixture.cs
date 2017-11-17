using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;

namespace SimpleUptime.IntegrationTests.Infrastructure.Repositories
{
    public class DocumentDbFixture : IDisposable
    {
        private const string EndpointUrl = "https://localhost:8081";
        private const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly DocumentClient _client;

        public DocumentDbFixture()
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new HttpMonitorIdJsonConverter());

            _client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey, settings);

            var script = new SimpleUptimeDbScript(_client);

            script.DropDatabaseAsync().Wait();
            script.ExecuteAsync().Wait();
        }

        public IDocumentClient DocumentClient => _client;

        public void Reset()
        {
            var script = new SimpleUptimeDbScript(_client);

            script.RecreateDocumentCollectionsAsync().Wait();
        }

        public void Dispose()
        {
            var script = new SimpleUptimeDbScript(_client);

            script.DropDatabaseAsync().Wait();

            _client.Dispose();
        }
    }
}