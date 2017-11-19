using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleUptime.Infrastructure.Repositories;

namespace SimpleUptime.WebApi
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder EnsureDocumentDatabase(this IApplicationBuilder app)
        {
            var script = app.ApplicationServices.GetService<SimpleUptimeDbScript>();

            script.ExecuteMigrationScript().Wait();

            return app;
        }
    }
}