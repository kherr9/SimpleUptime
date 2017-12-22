using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleUptime.IntegrationTests.Util.Client
{
    public static class HttpContentExtension
    {
        public static async Task<TModel> ReadAsJsonAsync<TModel>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TModel>(json);
        }
    }
}