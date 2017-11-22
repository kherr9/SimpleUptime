namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Client
{
    public struct Urls
    {
        public struct HttpMonitors
        {
            private const string BaseUrl = "/api/httpmonitors";

            public static string Get() => BaseUrl;

            public static string Get(string id) => $"{BaseUrl}/{id}";

            public static string Post() => BaseUrl;

            public static string Put(string id) => $"{BaseUrl}/{id}";

            public static string Delete(string id) => $"{BaseUrl}/{id}";

            public static string Test(string id) => $"{BaseUrl}/{id}/test";
        }
    }
}