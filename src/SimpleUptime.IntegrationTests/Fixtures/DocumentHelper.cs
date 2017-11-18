using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class DocumentHelper : IDisposable
    {
        private readonly DocumentClient _client;

        private DocumentHelper(DocumentClient client)
        {
            _client = client;
        }

        public async Task DeleteAllDocumentsAsync()
        {
            foreach (var db in _client.CreateDatabaseQuery().ToList())
                foreach (var coll in _client.CreateDocumentCollectionQuery(db.CollectionsLink).ToList())
                    foreach (var doc in _client.CreateDocumentQuery(coll.DocumentsLink).ToList())
                    {
                        await _client.DeleteDocumentAsync(doc.SelfLink);
                    }
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public static DocumentHelper Create()
        {
            var endpointUrl = "https://localhost:8081";
            var primaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            var client = new DocumentClient(new Uri(endpointUrl), primaryKey);

            client.OpenAsync().Wait();

            return new DocumentHelper(client);
        }
    }
}