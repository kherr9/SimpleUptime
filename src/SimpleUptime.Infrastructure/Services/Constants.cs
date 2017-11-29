using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleUptime.Infrastructure.JsonConverters;

namespace SimpleUptime.Infrastructure.Services
{
    public static class Constants
    {
        static Constants()
        {
            var settings = new JsonSerializerSettings
            {
                // The default value, DateParseHandling.DateTime, drops time zone information from DateTimeOffets.
                // This value appears to work well with both DateTimes (without time zone information) and DateTimeOffsets.
                DateParseHandling = DateParseHandling.DateTimeOffset,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            settings.Converters.Add(new GuidValueJsonConverter());
            settings.Converters.Add(new HttpMethodJsonConverter());

            JsonSerializerSettings = settings;
        }

        public static JsonSerializerSettings JsonSerializerSettings { get; }

        public struct Properties
        {
            public const string MessageTypeName = "MessageType-Name";
            public const string MessageTypeAssemblyQualifiedName = "MessageType-AssemblyQualifiedName";
        }
    }
}