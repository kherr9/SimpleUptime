using System;
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

        public async Task<HttpMonitorCheck> CheckHttpEndpointAsync(CheckHttpEndpoint command)
        {
            HttpResponse response = null;
            DateTime startTime;
            DateTime endTime;
            string errorMessage = null;

            using (var requestMessage = BuildRequestMessage(command))
            {
                startTime = DateTime.UtcNow;

                try
                {
                    using (var responseMessage = await SendMessageAsync(requestMessage))
                    {
                        response = new HttpResponse(responseMessage);
                    }
                }
                catch (HttpRequestException ex)
                {
                    if (ex.InnerException is Win32Exception win32Exception)
                    {
                        // A connection with the server could not be established
                        errorMessage = win32Exception.Message;
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                }
                catch (TaskCanceledException)
                {
                    errorMessage = "Request timed out";
                }
                finally
                {
                    endTime = DateTime.UtcNow;
                }
            }

            return command.CreateHttpMonitorCheck(new HttpRequestTiming(startTime, endTime), response, errorMessage);
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
