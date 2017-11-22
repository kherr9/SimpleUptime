using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace SimpleUptime.IntegrationTests.Fixtures
{
    /// <inheritdoc />
    /// <summary>
    /// Test http server with configurable request handler
    /// </summary>
    public class OpenHttpServer : IDisposable
    {
        private static int DefaultPort = 5050;
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

        public Uri BaseAddress { get; private set; }

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
            client.Timeout = TimeSpan.FromSeconds(15);

            _clients.Add(client);

            return client;
        }

        public static OpenHttpServer CreateAndRun()
        {
            var currentPort = DefaultPort;

            while (true)
            {
                OpenHttpServer server = null;
                try
                {
                    server = new OpenHttpServer();

                    var baseAddress = new Uri($"http://localhost:{currentPort}");
                    var webHost = WebHost.Start(baseAddress.ToString(), ctx => server.Handler(ctx));

                    server.Host = webHost;
                    server.BaseAddress = baseAddress;

                    return server;
                }
                catch (IOException ex)
                {
                    server?.Dispose();

                    if (!(ex.InnerException is AddressInUseException))
                    {
                        throw;
                    }
                }

                currentPort++;
            }
        }

        public void Dispose()
        {
            Host?.Dispose();
            _clients.ForEach(c => c.Dispose());
        }
    }
}
