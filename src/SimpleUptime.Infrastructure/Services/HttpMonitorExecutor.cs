using System.ComponentModel;
using System.Net.Http;
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

                try
                {
                    using (var responseMessage = await SendMessageAsync(requestMessage))
                    {
                        @event.Response = new HttpResponse(responseMessage);
                    }
                }
                catch (HttpRequestException ex)
                {
                    if (ex.InnerException is Win32Exception win32Exception)
                    {
                        // A connection with the server could not be established
                        @event.ErrorMessage = win32Exception.Message;
                    }
                    else
                    {
                        @event.ErrorMessage = ex.Message;
                    }
                }
                catch (TaskCanceledException)
                {
                    @event.ErrorMessage = "Request timed out";
                }
                finally
                {
                    @event.RequestTiming.SetEndTime();
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
            return _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
