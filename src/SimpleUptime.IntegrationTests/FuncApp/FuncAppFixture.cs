using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.WindowsAzure.Storage;
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
        private readonly HttpClient _httpClient;

        public FuncAppFixture()
        {
            _documentDbFixture = new DocumentDbFixture();
            _httpServer = OpenHttpServer.CreateAndRun();
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:7071")
            };
            // todo: delete container
        }

        public IDocumentClient DocumentClient => _documentDbFixture.DocumentClient;

        public IHttpMonitorRepository HttpMonitorRepository => new HttpMonitorDocumentRepository(DocumentClient, DatabaseConfigurations.Create());

        public IHttpMonitorCheckRepository HttpMonitorCheckRepository => new HttpMonitorCheckDocumentRepository(DocumentClient, DatabaseConfigurations.Create());

        public OpenHttpServer OpenHttpServer => _httpServer;

        public HttpClient HttpClient => _httpClient;

        public async Task StartHostAsync()
        {
            if (_process != null)
            {
                return;
                ////throw new InvalidOperationException("Process already started");
            }

            await ClearWebJobDataAsync();

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
            _httpClient?.Dispose();
        }

        private async Task ClearWebJobDataAsync()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("azure-webjobs-hosts");
            await container.DeleteIfExistsAsync();
        }

        public void Reset()
        {
            _documentDbFixture.Reset();
        }
    }
}