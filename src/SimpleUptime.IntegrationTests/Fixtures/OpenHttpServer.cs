using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    /// <inheritdoc />
    /// <summary>
    /// Test http server with configurable request handler
    /// </summary>
    public class OpenHttpServer : IDisposable
    {
        private static readonly Uri DefaultBaseAddress = new Uri("http://localhost:5051");
        private static readonly RequestDelegate NullRequestDelegate = ctx =>
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        };

        private readonly List<HttpClient> _clients = new List<HttpClient>();

        private OpenHttpServer()
        {
        }

        public IWebHost Host { get; private set; }

        public RequestDelegate Handler { get; set; } = NullRequestDelegate;

        public Uri BaseAddress { get; } = DefaultBaseAddress;

        public HttpClient CreateClient()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false,
                //ClientCertificateOptions = ClientCertificateOption.Manual,
                //PreAuthenticate = false,
                //UseProxy = false
            };

            var client = new HttpClient(handler, true)
            {
                BaseAddress = BaseAddress
            };
            client.DefaultRequestHeaders.ExpectContinue = false;
            client.DefaultRequestHeaders.ConnectionClose = true;

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
