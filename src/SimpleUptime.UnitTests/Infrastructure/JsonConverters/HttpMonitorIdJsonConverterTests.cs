using System;
using Newtonsoft.Json;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.JsonConverters;
using Xunit;

namespace SimpleUptime.UnitTests.Infrastructure.JsonConverters
{
    public class HttpMonitorIdJsonConverterTests
    {
        [Fact]
        public void SerializesHttpMonitorIdAsString()
        {
            // Arrange
            var entity = Entity.GeneratEntity();

            // Act
            var json = JsonConvert.SerializeObject(entity, Formatting.None, new HttpMonitorIdJsonConverter());

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
            var entity = JsonConvert.DeserializeObject<Entity>(json, new HttpMonitorIdJsonConverter());

            // Assert
            Assert.Equal(id, entity.Id.Value);
        }

        private class Entity
        {
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
