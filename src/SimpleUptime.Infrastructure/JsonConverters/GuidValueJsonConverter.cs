using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Infrastructure.JsonConverters
{
    /// <inheritdoc />
    /// <summary>
    /// Converts <see cref="T:SimpleUptime.Domain.Models.IGuidValue" /> into a json string of type Guid
    /// </summary>
    public class GuidValueJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IGuidValue id)
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
            if (CanConvert(objectType))
            {
                var token = JToken.Load(reader);

                if (token is JValue value)
                {
                    return Activator.CreateInstance(objectType, Guid.Parse(Convert.ToString(value.Value)));
                }

                throw new InvalidOperationException($"Unexpected token type {token.GetType().Name}");
            }

            throw new InvalidOperationException($"Unexpected type {objectType.Name}");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IGuidValue).IsAssignableFrom(objectType);
        }
    }
}
