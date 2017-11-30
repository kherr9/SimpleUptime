using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Infrastructure.JsonConverters
{
    public class AlertContactConverter : JsonConverter
    {
        private const string TypePropertyName = "type";

        // multiple readers is thread safe, but don't enumerate.
        private readonly Dictionary<string, Type> _types;

        public AlertContactConverter()
        {
            _types = typeof(IAlertContact).Assembly
                .GetTypes()
                .Where(t => t.IsPublic && t.IsClass && CanConvert(t))
                .ToDictionary(x => x.Name, x => x);
        }

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

            if (!_types.TryGetValue(typeValue, out var targetType))
            {
                throw new InvalidOperationException($"Unknown alert contact type '{typeValue}'");
            }

            var alertContact = Activator.CreateInstance(targetType);

            serializer.Populate(jsonObject.CreateReader(), alertContact);

            return alertContact;
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

        private static ExpandoObject CreateExpandoFromObject(object source)
        {
            var result = new ExpandoObject();
            IDictionary<string, object> dictionary = result;
            foreach (var property in source
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetMethod.IsPublic))
            {
                dictionary[property.Name] = property.GetValue(source, null);
            }

            return result;
        }
    }
}