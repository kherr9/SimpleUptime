using System;
using System.Runtime.Serialization.Formatters;
using Microsoft.Azure.Documents;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.Infrastructure.Services;

namespace SimpleUptime.MasterApp.Infrastructure
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var services = new ServiceCollection();
            RegisterServices(services);
            var serviceProvider = services.BuildServiceProvider(true);

            context
                .AddBindingRule<InjectAttribute>()
                .Bind(new InjectBindingProvider(serviceProvider));

            var registry = context.Config.GetService<IExtensionRegistry>();
            var filter = new ScopeCleanupFilter();
            registry.RegisterExtension(typeof(IFunctionInvocationFilter), filter);
            registry.RegisterExtension(typeof(IFunctionExceptionFilter), filter);
        }

        private void RegisterServices(IServiceCollection services)
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

            services.AddTransient<ICheckHttpEndpointPublisher, CheckHttpEndpointPublisher>();
            services.AddTransient<JsonSerializer>(provider =>
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
                };

                settings.Converters.Add(new HttpMonitorIdJsonConverter());
                settings.Converters.Add(new HttpMethodJsonConverter());

                return JsonSerializer.Create(settings);
            });
            services.AddTransient<ITopicClient, TopicClient>(provider => new TopicClient("Endpoint=sb://simpleuptime-dev-sbn.servicebus.windows.net/;SharedAccessKeyName=SendListenSharedAccessKey;SharedAccessKey=/Lz+rZ0A+EEdSuVoxPsnBa8x6tyf6kioHr4Rigbh+M8=", "master.events"));

            services.AddTransient<ICheckHttpMonitorPublisherService, CheckHttpMonitorPublisherService>();
        }
    }
}