using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace SimpleUptime.FuncApp.Infrastructure
{
    public static class AssemblyRedirectConfiguration
    {
        public static void Initialize(ExtensionConfigContext context)
        {
            var log = context.Trace;

            RedirectAssembly(new BindingRedirect("System.Diagnostics.TraceSource", "b03f5f7f11d50a3a", "4.0.2.0"), log);
            RedirectAssembly(new BindingRedirect("System.Collections.Specialized", "b03f5f7f11d50a3a", "4.0.3.0"), log);
            RedirectAssembly(new BindingRedirect("System.Collections.NonGeneric", "b03f5f7f11d50a3a", "4.0.3.0"), log);
        }

        private static void RedirectAssembly(BindingRedirect bindingRedirect, TraceWriter log)
        {
            Assembly Handler(object sender, ResolveEventArgs args)
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != bindingRedirect.ShortName)
                {
                    return null;
                }

                log.Info($"Found match for {bindingRedirect}");

                var targetPublicKeyToken = new AssemblyName("x, PublicKeyToken=" + bindingRedirect.PublicKeyToken).GetPublicKeyToken();
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);
                requestedAssembly.Version = new Version(bindingRedirect.RedirectToVersion);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;
                AppDomain.CurrentDomain.AssemblyResolve -= Handler;
                return Assembly.Load(requestedAssembly);
            }

            AppDomain.CurrentDomain.AssemblyResolve += Handler;
        }

        /// <summary>
        /// this was helpful in determining what version of assembly is loaded
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="log"></param>
        private static void TryLoadAssemblyInBin(string filename, TraceWriter log)
        {
            try
            {
                var path = Path.Combine(@"D:\home\site\wwwroot\bin", filename);
                log.Info($"Trying to load: {path}");
                var assembly = Assembly.LoadFile(path);
                log.Info($"Loaded assembly: {assembly.FullName}");
            }
            catch (Exception ex)
            {
                log.Error("Failed to load assembly.", ex);
            }
        }

        private static void PrintLoadedAssemblies(TraceWriter log)
        {
            foreach (var assemblies in AppDomain.CurrentDomain.GetAssemblies())
            {
                log.Info($"Previously Loaded: {assemblies.FullName}");
            }
        }

        private class BindingRedirect
        {
            public BindingRedirect(string shortName, string publicKeyToken, string redirectToVersion)
            {
                ShortName = shortName;
                PublicKeyToken = publicKeyToken;
                RedirectToVersion = redirectToVersion;
            }

            public string ShortName { get; }
            public string PublicKeyToken { get; }
            public string RedirectToVersion { get; }

            public override string ToString()
            {
                return $"{ShortName}, {PublicKeyToken}, {RedirectToVersion}";
            }
        }
    }
}