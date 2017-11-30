using System;
using System.Collections.Generic;

namespace SimpleUptime.Domain.Models
{
    public class AlertContactId : IGuidValue, IComparable<AlertContactId>
    {
        public AlertContactId(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("Empty guid not a valid value.", nameof(value));

            Value = value;
        }

        public Guid Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static AlertContactId Create()
        {
            return new AlertContactId(Guid.NewGuid());
        }

        public override bool Equals(object obj)
        {
            return obj is AlertContactId id
                   && Value.Equals(id.Value);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<Guid>.Default.GetHashCode(Value);
        }

        public static implicit operator Guid(AlertContactId id)
        {
            return id.Value;
        }

        public static implicit operator AlertContactId(Guid value)
        {
            return new AlertContactId(value);
        }

        public static bool operator ==(AlertContactId x, AlertContactId y)
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

        public static bool operator !=(AlertContactId x, AlertContactId y)
        {
            return !(x == y);
        }

        public int CompareTo(AlertContactId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Value.CompareTo(other.Value);
        }
    }
}