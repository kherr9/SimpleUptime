using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using ToyStorage;

namespace SimpleUptime.Infrastructure.Repositories
{
    /// <summary>
    /// Middleware component that ignores file not found.
    /// </summary>
    public class IgnoreNotFoundExceptionMiddleware : IMiddlewareComponent
    {
        private const int NotFound = 404;

        public async Task Invoke(RequestContext context, RequestDelegate next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode != NotFound)
                {
                    throw;
                }
            }
        }
    }
}
