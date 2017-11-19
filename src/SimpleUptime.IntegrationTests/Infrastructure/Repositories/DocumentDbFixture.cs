using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.IntegrationTests.Fixtures;

namespace SimpleUptime.IntegrationTests.Infrastructure.Repositories
{
    public class DocumentDbFixture : IDisposable
    {
        private readonly DocumentClient _client;
        private readonly DocumentHelper _documentHelper;

        public DocumentDbFixture()
        {
            _client = DocumentClientFactory.CreateDocumentClientForEmulatorAsync().Result;

            var script = new SimpleUptimeDbScript(_client);

            script.ExecuteAsync().Wait();

            _documentHelper = DocumentHelper.Create();
        }

        public IDocumentClient DocumentClient => _client;

        public void Reset()
        {
            _documentHelper.DeleteAllDocumentsAsync().Wait();
        }

        public void Dispose()
        {
            _documentHelper.DeleteAllDocumentsAsync().Wait();

            _client.Dispose();
        }
    }
}