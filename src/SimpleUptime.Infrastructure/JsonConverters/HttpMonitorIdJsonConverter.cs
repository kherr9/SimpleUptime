using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Infrastructure.JsonConverters
{
    public class HttpMonitorIdJsonConverter : JsonConverter
    {
        private static readonly Type Type = typeof(HttpMonitorId);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is HttpMonitorId id)
            {
                var token = JToken.FromObject(id.Value);
                token.WriteTo(writer);
            }
            else
            {
                throw new InvalidOperationException($"Unexpected type {value?.GetType().Name}");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == Type)
            {
                var token = JToken.Load(reader);

                if (token is JValue value)
                {
                    return new HttpMonitorId(Guid.Parse(Convert.ToString(value.Value)));
                }

                throw new InvalidOperationException($"Unexpected token type {token.GetType().Name}");
            }

            throw new InvalidOperationException($"Unexpected type {objectType.Name}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == Type;
        }
    }
}
