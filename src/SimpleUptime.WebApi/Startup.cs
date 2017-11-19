using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Repositories;
using SimpleUptime.Infrastructure.JsonConverters;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.WebApi.ModelBinders;
using SimpleUptime.WebApi.RouteConstraints;

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
                    options.ModelBinderProviders.Insert(0, new HttpMonitorIdBinder());
                })
                .AddJsonOptions(opt => opt.SerializerSettings.Converters.Add(new HttpMonitorIdJsonConverter()));

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add(HttpMonitorIdRouteConstraint.RouteLabel, typeof(HttpMonitorIdRouteConstraint));
            });

            services.AddOptions();
            services.Configure<DocumentClientSettings>(Configuration.GetSection("DocumentClientSettings"));

            services.AddTransient<IHttpMonitorService, HttpMonitorService>();

            services.AddTransient<IHttpMonitorRepository, HttpMonitorDocumentRepository>();
            services.AddSingleton<IDocumentClient>(provider => DocumentClientFactory.CreateDocumentClientAsync(provider.GetService<IOptions<DocumentClientSettings>>().Value).Result);
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
