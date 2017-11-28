using System;
using System.Net.Http;
using System.Runtime.Serialization.Formatters;
using Microsoft.Azure.Documents;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.FuncApp
{
    public static class Configurations
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // serivces
            services.AddTransient<IHttpMonitorRepository, HttpMonitorDocumentRepository>();
            services.AddSingleton<DocumentClientSettings>(provider => new DocumentClientSettings
            {
                ServiceEndpoint = new Uri("https://localhost:8081"),
                AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            });
            services.AddSingleton<IDocumentClient>(provider => DocumentClientFactory.CreateDocumentClientAsync(provider.GetService<DocumentClientSettings>()).Result);
            services.AddTransient<DatabaseConfigurations>(_ => DatabaseConfigurations.Create());

            services.AddTransient<ICheckHttpEndpointPublisher, CheckHttpEndpointQueuePublisher>();
            services.AddTransient<IHttpEndpointCheckedPublisher, HttpEndpointCheckedQueuePublisher>();
            services.AddTransient<JsonSerializer>(provider =>
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
                };

                settings.Converters.Add(new GuidValueJsonConverter());
                settings.Converters.Add(new HttpMethodJsonConverter());

                return JsonSerializer.Create(settings);
            });
            services.AddTransient<ITopicClient, TopicClient>(provider => new TopicClient("Endpoint=sb://simpleuptime-dev-sbn.servicebus.windows.net/;SharedAccessKeyName=SendListenSharedAccessKey;SharedAccessKey=/Lz+rZ0A+EEdSuVoxPsnBa8x6tyf6kioHr4Rigbh+M8=", "master.events"));

            services.AddTransient<ICheckHttpMonitorPublisherService, CheckHttpMonitorPublisherService>();

            services.AddTransient<CloudStorageAccount>(provider => CloudStorageAccount.Parse("UseDevelopmentStorage=true"));
            services.AddTransient<CloudQueueClient>(provider => provider.GetService<CloudStorageAccount>().CreateCloudQueueClient());

            services.AddTransient<IHttpMonitorExecutor, HttpMonitorExecutor>();
            services.AddSingleton<HttpClient>();

            services.AddTransient<CloudQueueFactory>();
            services.AddTransient<CreateCloudQueueAsync>(provider =>
                provider.GetService<CloudQueueFactory>().CreateCloudQueueAsync);
        }
    }
}
