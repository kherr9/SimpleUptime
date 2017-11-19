using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Infrastructure.JsonConverters;

namespace SimpleUptime.Infrastructure.Repositories
{
    public static class DocumentClientFactory
    {
        public static Task<DocumentClient> CreateDocumentClientAsync(DocumentClientSettings settings)
        {
            return CreateDocumentClientAsync(settings.ServiceEndpoint, settings.AuthKey);
        }

        /// <summary>
        /// Creates instance of <see cref="DocumentClient"/>
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="authKey"></param>
        /// <returns></returns>
        public static async Task<DocumentClient> CreateDocumentClientAsync(Uri serviceEndpoint, string authKey)
        {
            if (serviceEndpoint == null) throw new ArgumentNullException(nameof(serviceEndpoint));
            if (authKey == null) throw new ArgumentNullException(nameof(authKey));

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new HttpMonitorIdJsonConverter());

            var connectionPolicy = new ConnectionPolicy()
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };

            var client = new DocumentClient(serviceEndpoint, authKey, settings, connectionPolicy);

            await client.OpenAsync();

            return client;
        }
    }
}