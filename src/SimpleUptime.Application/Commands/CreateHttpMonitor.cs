using System;
using System.Net.Http;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Commands
{
    public class CreateHttpMonitor
    {
        public HttpRequest Request { get; set; }
    }
}