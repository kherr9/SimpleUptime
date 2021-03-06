﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.Domain.Models
{
    [DebuggerDisplay("{Method} {Url}")]
    public class HttpRequest
    {
        public HttpRequest(HttpMethod method, Uri url)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public HttpMethod Method { get; }

        public Uri Url { get; }

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