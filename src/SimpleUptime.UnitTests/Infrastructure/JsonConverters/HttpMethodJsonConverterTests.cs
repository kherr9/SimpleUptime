using System.Net.Http;
using Newtonsoft.Json;
using SimpleUptime.Infrastructure.JsonConverters;
using Xunit;

namespace SimpleUptime.UnitTests.Infrastructure.JsonConverters
{
    public class HttpMethodJsonConverterTests
    {
        [Fact]
        public void SerializeHttpMethodAsString()
        {
            // Arrange
            var entity = new Entity(HttpMethod.Delete);

            // Act
            var json = JsonConvert.SerializeObject(entity, Formatting.None, new HttpMethodJsonConverter());

            // Assert
            var expectedJson = JsonConvert.SerializeObject(new { Method = "DELETE" });
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void DeserializeHttpMethodAsString()
        {
            // Arrange
            var method = "PUT";
            var json = JsonConvert.SerializeObject(new { Method = method });

            // Act
            var entity = JsonConvert.DeserializeObject<Entity>(json, new HttpMethodJsonConverter());

            // Assert
            Assert.Equal(method, entity.Method.Method);
        }

        private class Entity
        {
            public Entity(HttpMethod method)
            {
                Method = method;
            }

            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            public HttpMethod Method { get; set; }
        }
    }
}
