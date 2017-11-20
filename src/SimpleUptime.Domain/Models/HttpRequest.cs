using System;
using System.Collections.Generic;
using System.Net.Http;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.Domain.Models
{
    public class HttpRequest
    {
        public Uri Url { get; set; }

        public HttpMethod Method { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HttpRequest request &&
                   EqualityComparer<Uri>.Default.Equals(Url, request.Url) &&
                   EqualityComparer<HttpMethod>.Default.Equals(Method, request.Method);
        }

        public override int GetHashCode()
        {
            var hashCode = 645682878;
            hashCode = hashCode * -1521134295 + EqualityComparer<Uri>.Default.GetHashCode(Url);
            hashCode = hashCode * -1521134295 + EqualityComparer<HttpMethod>.Default.GetHashCode(Method);
            return hashCode;
        }
    }
}