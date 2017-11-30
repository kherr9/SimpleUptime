using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.Domain.Models
{
    [DebuggerDisplay("{" + nameof(StatusCode) + "}")]
    public class HttpResponse
    {
        public HttpResponse(HttpStatusCode statusCode)
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
                throw new InvalidEnumArgumentException(nameof(statusCode), (int)statusCode, typeof(HttpStatusCode));

            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HttpResponse response &&
                   StatusCode == response.StatusCode;
        }

        public override int GetHashCode()
        {
            return -763886418 + StatusCode.GetHashCode();
        }

        public static HttpResponse Create(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            return new HttpResponse(responseMessage.StatusCode);
        }
    }
}