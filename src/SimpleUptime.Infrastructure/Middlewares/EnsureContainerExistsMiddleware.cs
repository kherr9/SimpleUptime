using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using ToyStorage;

namespace SimpleUptime.Infrastructure.Middlewares
{
    public class EnsureContainerExistsMiddleware : IMiddlewareComponent
    {
        private readonly BlobContainerPublicAccessType _accessType;
        private readonly Cache _cache = new Cache();

        public EnsureContainerExistsMiddleware(BlobContainerPublicAccessType accessType)
        {
            _accessType = accessType;
        }

        public async Task Invoke(RequestContext context, RequestDelegate next)
        {
            var cacheKey = $"{context.CloudBlockBlob.StorageUri}|{context.CloudBlockBlob.Container.Name}";
            if (!_cache.Exists(cacheKey))
            {
                await context.CloudBlockBlob.Container.CreateIfNotExistsAsync(_accessType, null, null, context.CancellationToken)
                    .ConfigureAwait(false);

                _cache.Add(cacheKey);
            }

            await next();
        }

        private sealed class Cache
        {
            private readonly ConcurrentDictionary<string, string> _containerNames = new ConcurrentDictionary<string, string>();

            public bool Exists(string key)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                return _containerNames.ContainsKey(key);
            }

            public void Add(string key)
            {
                _containerNames.TryAdd(key, null);
            }
        }
    }
}