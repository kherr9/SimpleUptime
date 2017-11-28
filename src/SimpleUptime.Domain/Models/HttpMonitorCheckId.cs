using System;
using System.Collections.Generic;

namespace SimpleUptime.Domain.Models
{
    public class HttpMonitorCheckId : IComparable<HttpMonitorCheckId>
    {
        public HttpMonitorCheckId(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("Empty guid not a valid value.", nameof(value));

            Value = value;
        }

        public Guid Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static HttpMonitorCheckId Create()
        {
            return new HttpMonitorCheckId(Guid.NewGuid());
        }

        public override bool Equals(object obj)
        {
            return obj is HttpMonitorCheckId id
                && Value.Equals(id.Value);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<Guid>.Default.GetHashCode(Value);
        }

        public static implicit operator Guid(HttpMonitorCheckId id)
        {
            return id.Value;
        }

        public static implicit operator HttpMonitorCheckId(Guid value)
        {
            return new HttpMonitorCheckId(value);
        }

        public static bool operator ==(HttpMonitorCheckId x, HttpMonitorCheckId y)
        {
            // Check for null on left side.
            if (ReferenceEquals(x, null))
            {
                if (ReferenceEquals(y, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return x.Equals(y);
        }

        public static bool operator !=(HttpMonitorCheckId x, HttpMonitorCheckId y)
        {
            return !(x == y);
        }

        public int CompareTo(HttpMonitorCheckId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Value.CompareTo(other.Value);
        }
    }
}
