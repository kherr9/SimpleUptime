using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Infrastructure.Middlewares;
using SimpleUptime.Infrastructure.Repositories;
using ToyStorage;
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
            services.AddMvc();

            services.AddTransient<IHttpMonitorService, HttpMonitorService>();
            services.AddTransient<IHttpMonitorRepository, HttpMonitorRepository>();
            services.AddTransient<IDocumentCollection, DocumentCollection>();
            services.AddTransient<CloudStorageAccount>(_ => CloudStorageAccount.Parse("UseDevelopmentStorage=true;"));
            services.AddTransient<CloudBlobClient>(provider => provider.GetService<CloudStorageAccount>().CreateCloudBlobClient());
            services.AddTransient<CloudBlobContainer>(provider => provider.GetService<CloudBlobClient>().GetContainerReference(nameof(HttpMonitor).ToLowerInvariant() + "s"));
            services.AddSingleton<EnsureContainerExistsMiddleware>(_ => new EnsureContainerExistsMiddleware(BlobContainerPublicAccessType.Off));
            services.AddTransient<IMiddlewarePipeline>(provider => new MiddlewarePipeline()
                .Use(provider.GetService<EnsureContainerExistsMiddleware>())
                .Use<IgnoreNotFoundExceptionMiddleware>()
                .UseJsonFormatter()
                .Use<BlobStorageMiddleware>());
            services.AddTransient<IDocumentCollection, DocumentCollection>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
