using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace SimpleUptime.Infrastructure.Repositories
{
    internal static class DocumentQueryExtension
    {
        public static async Task<T> FirstOrDefaultAsync<T>(this IDocumentQuery<T> query)
        {
            if (query.HasMoreResults)
            {
                return (await query.ExecuteNextAsync<T>()).FirstOrDefault();
            }

            return default(T);
        }

        public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this IDocumentQuery<T> query)
        {
            var result = new List<T>();

            while (query.HasMoreResults)
            {
                var batch = await query.ExecuteNextAsync<T>();

                result.AddRange(batch);
            }

            return result;
        }
    }
}