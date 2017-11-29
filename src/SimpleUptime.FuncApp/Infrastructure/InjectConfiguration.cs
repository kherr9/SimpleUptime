using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using SimpleUptime.Infrastructure.Repositories;

namespace SimpleUptime.FuncApp.Infrastructure
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            AssemblyRedirectConfiguration.Initialize(context);

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

            StartupTasks(serviceProvider);
        }

        private void RegisterServices(IServiceCollection services)
        {
            Configurations.RegisterServices(services);
        }

        private void StartupTasks(IServiceProvider serviceProvider)
        {
            var script = serviceProvider.GetService<SimpleUptimeDbScript>();

            script.ExecuteMigration().Wait();
        }
    }
}