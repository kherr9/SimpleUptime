using System;
using System.Collections.Generic;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Client
{
    public class HttpRequestDto
    {
        public Uri Url { get; set; }

        public string Method { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HttpRequestDto dto &&
                   EqualityComparer<Uri>.Default.Equals(Url, dto.Url) &&
                   Method == dto.Method;
        }

        public override int GetHashCode()
        {
            var hashCode = 645682878;
            hashCode = hashCode * -1521134295 + EqualityComparer<Uri>.Default.GetHashCode(Url);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Method);
            return hashCode;
        }
    }
}