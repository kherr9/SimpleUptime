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
            var db = _client.CreateDatabaseQuery().ToList().First();
            var coll = _client.CreateDocumentCollectionQuery(db.CollectionsLink).ToList().First();
            var docs = _client.CreateDocumentQuery(coll.DocumentsLink);

            foreach (var doc in docs)
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