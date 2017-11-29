using System.Diagnostics;
using System.Net;
using System.Net.Http;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.Domain.Models
{
    [DebuggerDisplay("{StatusCode}")]
    public class HttpResponse
    {
        public HttpResponse()
        {

        }

        public HttpResponse(HttpResponseMessage responseMessage)
        {
            StatusCode = responseMessage.StatusCode;
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
    }
}