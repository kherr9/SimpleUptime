using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization.Formatters;
using Microsoft.Azure.Documents;
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
// ReSharper disable RedundantTypeArgumentsOfMethod

namespace SimpleUptime.FuncApp
{
    public static class Configurations
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // serivces
            services.AddTransient<Settings>();
            services.AddTransient<IHttpMonitorRepository, HttpMonitorDocumentRepository>();
            services.AddTransient<IHttpMonitorCheckRepository, HttpMonitorCheckDocumentRepository>();

            services.AddSingleton<IDocumentClient>(provider => DocumentClientFactory.CreateDocumentClientAsync(provider.GetService<Settings>().ConnectionStrings.CosmosDb).Result);
            services.AddTransient<DatabaseConfigurations>(_ => DatabaseConfigurations.Create());
            services.AddTransient<SimpleUptimeDbScript>();

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

            services.AddTransient<ICheckHttpMonitorPublisherService, CheckHttpMonitorPublisherService>();

            services.AddTransient<CloudStorageAccount>(provider => CloudStorageAccount.Parse(provider.GetService<Settings>().ConnectionStrings.StorageAccount));
            services.AddTransient<CloudQueueClient>(provider => provider.GetService<CloudStorageAccount>().CreateCloudQueueClient());

            services.AddTransient<IHttpMonitorExecutor, HttpMonitorExecutor>();
            services.AddSingleton<HttpClient>();

            services.AddTransient<CloudQueueFactory>();
            services.AddTransient<CreateCloudQueueAsync>(provider =>
                provider.GetService<CloudQueueFactory>().CreateCloudQueueAsync);

            services.AddTransient<IHttpMonitorService, HttpMonitorService>();

            services.AddSingleton<JsonMediaTypeFormatter>(_ => new JsonMediaTypeFormatter
            {
                SerializerSettings = Constants.JsonSerializerSettings
            });
        }
    }
}
