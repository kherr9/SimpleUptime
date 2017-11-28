using System;
using System.Diagnostics;
using Microsoft.Azure.Documents;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.IntegrationTests.Fixtures;

namespace SimpleUptime.IntegrationTests.FuncApp
{
    public class FuncAppFixture : IDisposable
    {
        private Process _process;
        private readonly DocumentDbFixture _documentDbFixture;
        private readonly OpenHttpServer _httpServer;

        public FuncAppFixture()
        {
            _documentDbFixture = new DocumentDbFixture();
            _httpServer = OpenHttpServer.CreateAndRun();
            // todo: delete container
        }

        public IDocumentClient DocumentClient => _documentDbFixture.DocumentClient;

        public IHttpMonitorRepository HttpMonitorRepository => new HttpMonitorDocumentRepository(DocumentClient, DatabaseConfigurations.Create());

        public IHttpMonitorCheckRepository HttpMonitorCheckRepository => new HttpMonitorCheckDocumentRepository(DocumentClient, DatabaseConfigurations.Create());

        public OpenHttpServer OpenHttpServer => _httpServer;

        public void StartHost()
        {
            if (_process != null)
            {
                throw new InvalidOperationException("Process already started");
            }

            var fileName = @"C:\Users\kherr\AppData\Local\Azure.Functions.Cli\1.0.7\func.exe";
            var args = "host start";

#if DEBUG
            var workingDirectory = "Debug";
#else
            var workingDirectory = "Release";
#endif
            _process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = fileName,
                    Arguments = args,
                    WorkingDirectory = @"C:\git\SimpleUptime\src\SimpleUptime.FuncApp\bin\"+ workingDirectory + @"\net461"
                }
            };

            if (!_process.Start())
            {
                throw new Exception("Start process returned error");
            }
        }

        public void Dispose()
        {
            _process?.CloseMainWindow();
            _process?.Close();
            _process?.Dispose();
            _documentDbFixture?.Dispose();
            _httpServer?.Dispose();
        }
    }
}