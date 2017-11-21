using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    public class OpenHttpServer : IDisposable
    {
        private static readonly Uri DefaultBaseAddress = new Uri("http://localhost:5051");
        private static readonly RequestDelegate NullRequestDelegate = ctx => Task.CompletedTask;

        private readonly List<HttpClient> _clients = new List<HttpClient>();

        public IWebHost Host { get; private set; }

        public RequestDelegate Handler { get; set; } = NullRequestDelegate;

        public Uri BaseAddress { get; } = DefaultBaseAddress;

        public HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                BaseAddress = BaseAddress
            };

            _clients.Add(client);

            return client;
        }

        public static OpenHttpServer CreateAndRun()
        {
            var server = new OpenHttpServer();

            server.Host = WebHost.Start(server.BaseAddress.ToString(), ctx => server.Handler(ctx));

            return server;
        }

        public void Dispose()
        {
            Host?.Dispose();
            _clients.ForEach(c => c.Dispose());
        }
    }
}
