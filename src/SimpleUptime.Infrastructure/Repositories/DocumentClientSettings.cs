using System;

namespace SimpleUptime.Infrastructure.Repositories
{
    public class DocumentClientSettings
    {
        public static readonly DocumentClientSettings Emulator = new DocumentClientSettings()
        {
            ServiceEndpoint = new Uri("https://localhost:8081"),
            AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
        };

        public Uri ServiceEndpoint { get; set; }

        public string AuthKey { get; set; }
    }
}
