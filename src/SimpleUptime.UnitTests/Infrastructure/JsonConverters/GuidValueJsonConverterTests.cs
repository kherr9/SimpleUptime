using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.JsonConverters;
using Xunit;

namespace SimpleUptime.UnitTests.Infrastructure.JsonConverters
{
    public class GuidValueJsonConverterTests
    {
        [Fact]
        public void SerializesHttpMonitorIdAsString()
        {
            // Arrange
            var entity = Entity.GeneratEntity();

            // Act
            var json = JsonConvert.SerializeObject(entity, Formatting.None, new GuidValueJsonConverter());

            // Assert
            var expectedJson = JsonConvert.SerializeObject(new { Id = entity.Id.Value });
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void DeserializesHttpMonitorIdAsString()
        {
            // Arrange
            var id = Guid.NewGuid();
            var json = JsonConvert.SerializeObject(new { Id = id });

            // Act
            var entity = JsonConvert.DeserializeObject<Entity>(json, new GuidValueJsonConverter());

            // Assert
            Assert.Equal(id, entity.Id.Value);
        }

        [Theory]
        [MemberData(nameof(GuidValueTypes))]
        public void SerializeAllIGuidValueType(Type valueType)
        {
            // Arrange
            var value = Guid.NewGuid();
            var entity = (IGuidValue)Activator.CreateInstance(valueType, value);

            // Act
            var json = JsonConvert.SerializeObject(entity, Formatting.None, new GuidValueJsonConverter());

            // Assert
            var expectedJson = JsonConvert.SerializeObject(value);
            Assert.Equal(expectedJson, json);
        }

        [Theory]
        [MemberData(nameof(GuidValueTypes))]
        public void DeserializeAllIGuidValueType(Type valueType)
        {
            // Arrange
            var value = Guid.NewGuid();
            var json = JsonConvert.SerializeObject(value);

            // Act
            var entity = (IGuidValue)JsonConvert.DeserializeObject(json, valueType, new GuidValueJsonConverter());

            // Assert
            Assert.Equal(value, entity.Value);
        }

        public static IEnumerable<object[]> GuidValueTypes()
        {
            var types = typeof(HttpMonitorId).Assembly.GetTypes()
                .Where(x => typeof(IGuidValue).IsAssignableFrom(x))
                .Where(x => x.IsPublic && x.IsClass)
                .ToList();

            if (!types.Any())
            {
                throw new InvalidOperationException("Expected a IGuidValue type");
            }

            return types.Select(x => new object[] { x });
        }

        private class Entity
        {
            // ReSharper disable once MemberCanBePrivate.Local
            public HttpMonitorId Id { get; set; }

            public static Entity GeneratEntity()
            {
                return new Entity()
                {
                    Id = HttpMonitorId.Create()
                };
            }
        }
    }
}
