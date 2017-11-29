using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUptime.FuncApp
{
    public class Settings
    {
        public ConnectionStrings ConnectionStrings { get; } = new ConnectionStrings();
    }

    public class ConnectionStrings
    {
        public string CosmosDb => GetConnectionString("CosmosDb");

        public string StorageAccount => GetConnectionString("StorageAccount");

        private string GetConnectionString(string key)
        {
            var connection = ConfigurationManager.ConnectionStrings[key];

            if (connection == null)
            {
                throw new Exception($"Missing connection string of {key}");
            }

            return connection.ConnectionString;
        }
    }
}
