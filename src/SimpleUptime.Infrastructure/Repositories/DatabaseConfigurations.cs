using System;
using Microsoft.Azure.Documents.Client;

namespace SimpleUptime.Infrastructure.Repositories
{
    public class DatabaseConfigurations
    {
        private DatabaseConfigurations()
        {
        }

        public string DatabaseId => "SimpleUptimeDb";

        public Uri DatabaseUri => UriFactory.CreateDatabaseUri(DatabaseId);

        public string DocumentCollectionId => "Entities";

        public Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri(DatabaseId, DocumentCollectionId);

        public Uri DocumentUri(string documentId) =>
            UriFactory.CreateDocumentUri(DatabaseId, DocumentCollectionId, documentId);

        public static DatabaseConfigurations Create()
        {
            return new DatabaseConfigurations();
        }
    }
}