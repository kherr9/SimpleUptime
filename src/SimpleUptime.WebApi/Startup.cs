using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.WebApi.ModelBinders;
using SimpleUptime.WebApi.RouteConstraints;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace SimpleUptime.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.ModelBinderProviders.Insert(0, new HttpMonitorIdBinder());
                })
                .AddJsonOptions(opt => opt.SerializerSettings.Converters.Add(new HttpMonitorIdJsonConverter()));

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add(HttpMonitorIdRouteConstraint.RouteLabel, typeof(HttpMonitorIdRouteConstraint));
            });

            services.AddTransient<IHttpMonitorService, HttpMonitorService>();

            services.AddTransient<IHttpMonitorRepository, HttpMonitorDocumentRepository>();
            services.AddSingleton<IDocumentClient>(provider =>
            {
                var endpointUrl = "https://localhost:8081";
                var primaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                settings.Converters.Add(new HttpMonitorIdJsonConverter());

                var connectionPolicy = new ConnectionPolicy()
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                };

                var client = new DocumentClient(new Uri(endpointUrl), primaryKey, settings, connectionPolicy);

                client.OpenAsync().Wait();

                return client;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.EnsureDocumentDatabase();
        }
    }
}
