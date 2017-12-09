using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Domain.Services;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.Infrastructure.Services;
using SimpleUptime.WebApi.ModelBinders;
using SimpleUptime.WebApi.RouteConstraints;
// ReSharper disable RedundantTypeArgumentsOfMethod

namespace SimpleUptime.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                // model binders
                options.ModelBinderProviders.Insert(0, new HttpMonitorIdBinder());
            })
            // mvc json settings
            .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.Converters.Add(new GuidValueJsonConverter());
                    opt.SerializerSettings.Converters.Add(new HttpMethodJsonConverter());
                });

            services.Configure<RouteOptions>(options =>
            {
                // route constraints
                options.ConstraintMap.Add(HttpMonitorIdRouteConstraint.RouteLabel, typeof(HttpMonitorIdRouteConstraint));
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAnyOrigin"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder => builder.AllowAnyOrigin());
            });

            // settings
            services.AddOptions();
            services.Configure<DocumentClientSettings>(Configuration.GetSection("DocumentClientSettings"));

            // serivces
            services.AddTransient<IHttpMonitorService, HttpMonitorService>();
            services.AddTransient<IHttpMonitorRepository, HttpMonitorDocumentRepository>();
            services.AddSingleton<IDocumentClient>(provider => DocumentClientFactory.CreateDocumentClientAsync(provider.GetService<IOptions<DocumentClientSettings>>().Value).Result);
            services.AddTransient<DatabaseConfigurations>(_ => DatabaseConfigurations.Create());
            services.AddTransient<SimpleUptimeDbScript>();

            services.AddTransient<IHttpMonitorExecutorService, HttpMonitorExecutorService>();
            services.AddTransient<IHttpMonitorExecutor, HttpMonitorExecutor>();
            services.AddSingleton<HttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseCors("AllowAnyOrigin");

            app.EnsureDocumentDatabase();
        }
    }
}
