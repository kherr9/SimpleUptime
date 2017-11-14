﻿using System;
using System.Collections.Generic;

namespace SimpleUptime.Domain.Models
{
    public class HttpMonitor
    {
        public HttpMonitorId Id { get; set; }

        public Uri Endpoint { get; set; }

        public int ExpectedStatusCode { get; set; }
    }

    public class HttpMonitorId
    {
        public HttpMonitorId(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("Empty guid not a valid value.", nameof(value));

            Value = value;
        }

        public Guid Value { get; }

        public string GetString()
        {
            return Value.ToString();
        }

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
            var id = obj as HttpMonitorId;
            return id != null &&
                   Value.Equals(id.Value);
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
    }
}
