using System;
using System.Net.Http;

namespace SimpleUptime.Domain.Models
{
    public class HttpRequest
    {
        public Uri Url { get; set; }

        public HttpMethod Method { get; set; }
    }
}