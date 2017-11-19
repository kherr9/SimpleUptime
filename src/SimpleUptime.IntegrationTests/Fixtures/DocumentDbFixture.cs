using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SimpleUptime.Infrastructure.Repositories;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class DocumentDbFixture : IDisposable
    {
        private readonly DocumentClient _client;
        private readonly DocumentHelper _documentHelper;

        public DocumentDbFixture()
        {
            _client = DocumentClientFactory.CreateDocumentClientAsync(DocumentClientSettings.Emulator).Result;

            var script = new SimpleUptimeDbScript(_client, DatabaseConfigurations.Create());

            script.DropDatabaseAsync().Wait();
            script.ExecuteMigrationScript().Wait();

            _documentHelper = new DocumentHelper(_client);
        }

        public IDocumentClient DocumentClient => _client;

        public void Dispose()
        {
            Reset();

            _client?.Dispose();
        }

        public void Reset()
        {
            _documentHelper.DeleteAllDocumentsAsync().Wait();
        }
    }
}