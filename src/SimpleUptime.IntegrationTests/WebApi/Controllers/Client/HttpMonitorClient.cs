using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Client
{
    public class HttpMonitorClient
    {
        private readonly HttpClient _client;

        public HttpMonitorClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<(HttpResponseMessage, HttpMonitorDto[])> GetAsync()
        {
            var response = await _client.GetAsync(Urls.HttpMonitors.Get());
            var model = await DeserializeOrDefaultAsync<HttpMonitorDto[]>(response);

            return (response, model);
        }

        public async Task<(HttpResponseMessage, HttpMonitorDto)> GetAsync(string id)
        {
            var response = await _client.GetAsync(Urls.HttpMonitors.Get(id));
            var model = await DeserializeOrDefaultAsync<HttpMonitorDto>(response);

            return (response, model);
        }

        public async Task<(HttpResponseMessage, HttpMonitorDto)> PostAsync(object entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Urls.HttpMonitors.Post(), content);

            var model = await DeserializeOrDefaultAsync<HttpMonitorDto>(response);

            return (response, model);
        }

        public async Task<(HttpResponseMessage, HttpMonitorDto)> PutAsync(string id, object entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(Urls.HttpMonitors.Put(id), content);

            var model = await DeserializeOrDefaultAsync<HttpMonitorDto>(response);

            return (response, model);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            return await _client.DeleteAsync(Urls.HttpMonitors.Delete(id));
        }

        public async Task<(HttpResponseMessage, HttpMonitorCheckedDto)> TestAsync(string id)
        {
            var response = await _client.PostAsync(Urls.HttpMonitors.Test(id), null);
            var model = await DeserializeOrDefaultAsync<HttpMonitorCheckedDto>(response);

            return (response, model);
        }

        private static async Task<T> DeserializeOrDefaultAsync<T>(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                return await httpResponseMessage.Content.ReadAsJsonAsync<T>();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}