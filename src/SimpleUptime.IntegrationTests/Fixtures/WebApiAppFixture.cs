using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Documents.Client;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.WebApi;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class WebApiAppFixture : IDisposable
    {
        private readonly TestServer _server;
        private readonly DocumentHelper _documentHelper;
        private readonly DocumentClient _client;

        public WebApiAppFixture()
        {
            _client = DocumentClientFactory.CreateDocumentClientAsync(DocumentClientSettings.Emulator).Result;

            _documentHelper = new DocumentHelper(_client);

            new SimpleUptimeDbScript(_client).DropDatabaseAsync().Wait();

            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            HttpClient = _server.CreateClient();
        }

        public HttpClient HttpClient { get; }

        public void Dispose()
        {
            Reset();

            _server?.Dispose();
            HttpClient?.Dispose();
            _client?.Dispose();
        }

        public void Reset()
        {
            _documentHelper.DeleteAllDocumentsAsync().Wait();
        }
    }
}
