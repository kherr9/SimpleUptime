using System;
using Microsoft.CSharp.RuntimeBinder;
using SimpleUptime.Domain.Models;
using Xunit;

namespace SimpleUptime.UnitTests.Domain.Models
{
    public class HttpMonitorIdTests
    {
        [Fact]
        public void EmptyGuidThrowsException()
        {
            // Arrange
            var value = Guid.Empty;

            // Act
            var exception = Assert.Throws<ArgumentException>(() => new HttpMonitorId(value));

            // Assert
            Assert.Equal("value", exception.ParamName);
        }

        [Fact]
        public void ValueIsImmutable()
        {
            // Arrange
            var id = HttpMonitorId.Create();
            var value = id.Value;
            var newValue = Guid.NewGuid();

            // Act
            // cast to dynamic to attempt to set value to get around compilation error
            Assert.Throws<RuntimeBinderException>(() => ((dynamic)id).Value = newValue);

            // Assert
            Assert.Equal(value, id.Value);
        }

        [Fact]
        public void CreateReturnsUniqueInstances()
        {
            // Act
            var id1 = HttpMonitorId.Create();
            var id2 = HttpMonitorId.Create();

            // Assert
            Assert.NotEqual(id1.Value, id2.Value);
        }
    }
}
