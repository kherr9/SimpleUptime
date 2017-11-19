using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SimpleUptime.Infrastructure.Repositories
{
    public class SimpleUptimeDbScript
    {
        private readonly IDocumentClient _client;
        private const string DatabaseId = "SimpleUptimeDb";
        private const string DocumentCollectionId = "Entities";

        public SimpleUptimeDbScript(IDocumentClient client)
        {
            _client = client;
        }

        public async Task ExecuteAsync()
        {
            await EnsureDatabaseAsync();

            await EnsureDocumentCollectionAsync();
        }

        public async Task DropDatabaseAsync()
        {
            var databaseUri = UriFactory.CreateDatabaseUri(DatabaseId);

            try
            {
                await _client.DeleteDatabaseAsync(databaseUri);
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }
        
        private async Task EnsureDatabaseAsync()
        {
            var database = new Database
            {
                Id = DatabaseId
            };

            await _client.CreateDatabaseIfNotExistsAsync(database);
        }

        private Task EnsureDocumentCollectionAsync()
        {
            var databaseUri = UriFactory.CreateDatabaseUri(DatabaseId);

            var documentCollection = new DocumentCollection
            {
                Id = DocumentCollectionId
            };

            return _client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection);
        }
    }
}