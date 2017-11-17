﻿using System;
using System.Collections.Generic;

namespace SimpleUptime.Domain.Models
{
    /// <summary>
    /// Identifier of <see cref="HttpMonitor"/>
    /// </summary>
    public class HttpMonitorId: IComparable<HttpMonitorId>
    {
        public HttpMonitorId(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("Empty guid not a valid value.", nameof(value));

            Value = value;
        }

        public Guid Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static HttpMonitorId Create()
        {
            return new HttpMonitorId(Guid.NewGuid());
        }

        public override bool Equals(object obj)
        {
            return obj is HttpMonitorId id
                && Value.Equals(id.Value);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<Guid>.Default.GetHashCode(Value);
        }

        public static implicit operator Guid(HttpMonitorId id)
        {
            return id.Value;
        }

        public static implicit operator HttpMonitorId(Guid value)
        {
            return new HttpMonitorId(value);
        }

        public static bool operator ==(HttpMonitorId x, HttpMonitorId y)
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

        public static bool operator !=(HttpMonitorId x, HttpMonitorId y)
        {
            return !(x == y);
        }

        public int CompareTo(HttpMonitorId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Value.CompareTo(other.Value);
        }
    }
}
