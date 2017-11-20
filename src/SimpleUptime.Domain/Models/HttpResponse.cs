using System.Net;
using System.Net.Http;

namespace SimpleUptime.Domain.Models
{
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
    }
}