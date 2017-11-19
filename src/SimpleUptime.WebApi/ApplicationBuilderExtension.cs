using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.DependencyInjection;
using SimpleUptime.Infrastructure.Repositories;

namespace SimpleUptime.WebApi
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder EnsureDocumentDatabase(this IApplicationBuilder app)
        {
            var client = app.ApplicationServices.GetService<IDocumentClient>();

            var script = new SimpleUptimeDbScript(client);
            script.ExecuteAsync().Wait();

            return app;
        }
    }
}