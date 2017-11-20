using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class DocumentHelper
    {
        private readonly DocumentClient _client;

        public DocumentHelper(DocumentClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task DeleteAllDocumentsAsync()
        {
            foreach (var db in _client.CreateDatabaseQuery().ToList())
                foreach (var coll in _client.CreateDocumentCollectionQuery(db.CollectionsLink).ToList())
                    foreach (var doc in _client.CreateDocumentQuery(coll.DocumentsLink).ToList())
                    {
                        try
                        {
                            await _client.DeleteDocumentAsync(doc.SelfLink);
                        }
                        catch (Exception ex)
                        {
                            // FileNotFound
                            Console.WriteLine(ex.ToString());
                        }
                    }
        }
    }
}