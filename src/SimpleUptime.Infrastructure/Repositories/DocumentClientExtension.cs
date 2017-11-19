using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace SimpleUptime.Infrastructure.Repositories
{
    public static class DocumentClientExtension
    {
        public static async Task CreateDatabaseIfNotExistsAsync(this IDocumentClient client, Database database)
        {
            try
            {
                await client.CreateDatabaseAsync(database);
            }
            catch (DocumentClientException ex)
            {
                if (!(ex.StatusCode == HttpStatusCode.Conflict && ex.Message.Contains("Resource with specified id or name already exists")))
                {
                    throw;
                }
            }
        }

        public static async Task CreateDocumentCollectionIfNotExistsAsync(this IDocumentClient client, Uri databaseId, DocumentCollection documentCollection)
        {
            try
            {
                await client.CreateDocumentCollectionAsync(databaseId, documentCollection);
            }
            catch (DocumentClientException ex)
            {
                if (!(ex.StatusCode == HttpStatusCode.Conflict && ex.Message.Contains("Resource with specified id or name already exists")))
                {
                    throw;
                }
            }
        }
    }
}