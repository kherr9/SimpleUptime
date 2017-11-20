﻿using System.Net.Http;
using System.Threading.Tasks;
using SimpleUptime.Domain.Commands;
using SimpleUptime.Domain.Events;
using SimpleUptime.Domain.Models;
using SimpleUptime.Domain.Services;

namespace SimpleUptime.Infrastructure.Services
{
    public class HttpMonitorExecutor : IHttpMonitorExecutor
    {
        private readonly HttpClient _httpClient;

        public HttpMonitorExecutor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpEndpointChecked> CheckHttpEndpointAsync(CheckHttpEndpoint command)
        {
            var @event = new HttpEndpointChecked()
            {
                HttpMonitorId = command.HttpMonitorId,
                Request = command.Request,
                Response = null,
                RequestTiming = new HttpRequestTiming()
            };

            using (var requestMessage = BuildRequestMessage(command))
            {
                @event.RequestTiming.SetStartTime();

                using (var responseMessage = await SendMessageAsync(requestMessage))
                {
                    await ReadResponseAsync(responseMessage);

                    @event.RequestTiming.SetEndTime();

                    @event.Response = new HttpResponse(responseMessage);
                }
            }

            return @event;
        }

        private HttpRequestMessage BuildRequestMessage(CheckHttpEndpoint command)
        {
            return new HttpRequestMessage(command.Request.Method, command.Request.Url);
        }

        private Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage requestMessage)
        {
            return _httpClient.SendAsync(requestMessage);
        }

        private Task ReadResponseAsync(HttpResponseMessage responseMessage)
        {
            return responseMessage.Content.ReadAsByteArrayAsync();
        }
    }
}
