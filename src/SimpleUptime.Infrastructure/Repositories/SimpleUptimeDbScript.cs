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
            await CreateDatabaseAsync();

            await CreateCollectionAsync();
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

        public async Task RecreateDocumentCollectionsAsync()
        {
            await DeleteCollectionAsync();

            await CreateCollectionAsync();
        }

        private Task CreateDatabaseAsync()
        {
            var database = new Database
            {
                Id = DatabaseId
            };

            return _client.CreateDatabaseAsync(database);
        }

        private Task DeleteCollectionAsync()
        {
            var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, DocumentCollectionId);

            return _client.DeleteDocumentCollectionAsync(documentCollectionUri);
        }

        private Task CreateCollectionAsync()
        {
            var databaseUri = UriFactory.CreateDatabaseUri(DatabaseId);

            var documentCollection = new DocumentCollection
            {
                Id = DocumentCollectionId
            };

            return _client.CreateDocumentCollectionAsync(databaseUri, documentCollection);
        }
    }
}