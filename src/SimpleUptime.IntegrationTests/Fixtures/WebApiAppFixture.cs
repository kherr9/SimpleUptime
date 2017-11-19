using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using SimpleUptime.WebApi;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class WebApiAppFixture : IDisposable
    {
        private readonly TestServer _server;
        private readonly DocumentHelper _documentHelper;

        public WebApiAppFixture()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _documentHelper = DocumentHelper.Create();

            HttpClient = _server.CreateClient();
        }

        public HttpClient HttpClient { get; }

        public void Dispose()
        {
            _server?.Dispose();
            HttpClient?.Dispose();
        }

        public void Reset()
        {
            _documentHelper.DeleteAllDocumentsAsync().Wait();
        }
    }
}
