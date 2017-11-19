using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace SimpleUptime.Infrastructure.Repositories
{
    public class SimpleUptimeDbScript
    {
        private readonly IDocumentClient _client;
        private readonly DatabaseConfigurations _configs;

        public SimpleUptimeDbScript(IDocumentClient client, DatabaseConfigurations configs)
        {
            _client = client;
            _configs = configs;
        }

        public async Task ExecuteMigration()
        {
            await EnsureDatabaseAsync();

            await EnsureDocumentCollectionAsync();
        }

        public async Task DropDatabaseAsync()
        {
            var databaseUri = _configs.DatabaseUri;

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
                Id = _configs.DatabaseId
            };

            await _client.CreateDatabaseIfNotExistsAsync(database);
        }

        private Task EnsureDocumentCollectionAsync()
        {
            var databaseUri = _configs.DatabaseUri;

            var documentCollection = new DocumentCollection
            {
                Id = _configs.DocumentCollectionId
            };

            return _client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection);
        }
    }
}