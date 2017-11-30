using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Infrastructure.JsonConverters;

namespace SimpleUptime.Infrastructure.Repositories
{
    public static class DocumentClientFactory
    {
        public static Task<DocumentClient> CreateDocumentClientAsync(string connectionString)
        {
            (var serviceEndpoint, var authKey) = DocumentDbAccount.ParseConnectionString(connectionString);
            return CreateDocumentClientAsync(new DocumentClientSettings() { ServiceEndpoint = serviceEndpoint, AuthKey = authKey });
        }

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
            settings.Converters.Add(new GuidValueJsonConverter());
            settings.Converters.Add(new HttpMethodJsonConverter());

            var connectionPolicy = new ConnectionPolicy()
            {
                // Direct connect does not work with emulator
                ////ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };

            var client = new DocumentClient(serviceEndpoint, authKey, settings, connectionPolicy);

            await client.OpenAsync();

            return client;
        }
    }

    internal static class DocumentDbAccount
    {
        private const string AccountEndpointKey = "AccountEndpoint";
        private const string AccountKeyKey = "AccountKey";
        private static readonly HashSet<string> RequireSettings = new HashSet<string>(new[] { AccountEndpointKey, AccountKeyKey }, StringComparer.OrdinalIgnoreCase);

        internal static (Uri, string) ParseConnectionString(string connectionString)
        {
            var settings = ParseStringIntoSettings(connectionString);

            if (!RequireSettings.IsSubsetOf(settings.Keys))
            {
                throw new Exception("Missing required fields");
            }

            return (new Uri(settings[AccountEndpointKey]), settings[AccountKeyKey]);
        }

        /// <summary>
        /// Tokenizes input and stores name value pairs.
        /// </summary>
        /// <param name="connectionString">The string to parse.</param>
        /// <param name="error">Error reporting delegate.</param>
        /// <returns>Tokenized collection.</returns>
        private static IDictionary<string, string> ParseStringIntoSettings(string connectionString)
        {
            IDictionary<string, string> settings = new Dictionary<string, string>();
            string[] splitted = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string nameValue in splitted)
            {
                string[] splittedNameValue = nameValue.Split(new char[] { '=' }, 2);

                if (splittedNameValue.Length != 2)
                {
                    throw new Exception("Settings must be of the form \"name=value\".");
                }

                if (settings.ContainsKey(splittedNameValue[0]))
                {
                    throw new Exception($"Duplicate setting '{splittedNameValue[0]}' found.");
                }

                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            return settings;
        }
    }

}