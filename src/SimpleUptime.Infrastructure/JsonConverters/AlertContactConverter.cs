using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Infrastructure.JsonConverters
{
    public class AlertContactConverter : JsonConverter
    {
        private const string TypePropertyName = "type";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = CreateDictionary(value);

            dict[TypePropertyName] = value.GetType().Name;

            serializer.Serialize(writer, dict);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (!jsonObject.TryGetValue(TypePropertyName, StringComparison.InvariantCultureIgnoreCase, out var jsonValue))
            {
                throw new InvalidOperationException($"Expected property name '{TypePropertyName}'.");
            }

            var typeValue = jsonValue.Value<string>();

            switch (typeValue)
            {
                case nameof(EmailAlertContact):
                    return new EmailAlertContact(
                        jsonObject.GetValue("id", StringComparison.InvariantCultureIgnoreCase).ToObject<AlertContactId>(serializer),
                        jsonObject.GetValue("email", StringComparison.InvariantCultureIgnoreCase).ToObject<string>(serializer)
                    );
                case nameof(SlackAlertContact):
                    return new SlackAlertContact(
                        jsonObject.GetValue("id", StringComparison.InvariantCultureIgnoreCase).ToObject<AlertContactId>(serializer),
                        jsonObject.GetValue("webHookUrl", StringComparison.InvariantCultureIgnoreCase).ToObject<Uri>(serializer)
                    );
                default:
                    throw new InvalidOperationException($"Unknown alert contact type of '{typeValue}'");
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IAlertContact).IsAssignableFrom(objectType);
        }

        private static Dictionary<string, object> CreateDictionary(object source)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var property in source
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetMethod.IsPublic))
            {
                dictionary[property.Name] = property.GetValue(source, null);
            }

            return dictionary;
        }
    }
}