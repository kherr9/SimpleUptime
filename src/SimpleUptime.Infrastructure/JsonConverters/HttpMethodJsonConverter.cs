using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleUptime.Infrastructure.JsonConverters
{
    public class HttpMethodJsonConverter : JsonConverter
    {
        private static readonly Type Type = typeof(HttpMethod);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is HttpMethod method)
            {
                var token = JToken.FromObject(method.Method, serializer);
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
                    return new HttpMethod(Convert.ToString(value.Value));
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