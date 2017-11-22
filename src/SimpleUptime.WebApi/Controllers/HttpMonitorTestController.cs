using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleUptime.Application.Exceptions;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.WebApi.Controllers
{
    [Route("api/httpmonitors")]
    public class HttpMonitorTestController : Controller
    {
        private readonly IHttpMonitorExecutorService _service;

        public HttpMonitorTestController(IHttpMonitorExecutorService service)
        {
            _service = service;
        }

        [HttpPost, Route("{httpMonitorId:HttpMonitorId}/test")]
        public async Task<IActionResult> Test(HttpMonitorId httpMonitorId)
        {
            try
            {
                var result = await _service.ExecuteAsync(httpMonitorId);

                // maybe set status code if considered "down"?
                return Ok(result);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}