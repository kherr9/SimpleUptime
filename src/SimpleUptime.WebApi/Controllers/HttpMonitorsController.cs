using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleUptime.Application.Commands;
using SimpleUptime.Application.Exceptions;
using SimpleUptime.Application.Services;
using SimpleUptime.Domain.Models;
using SimpleUptime.WebApi.ModelBinders;

namespace SimpleUptime.WebApi.Controllers
{
    [Route("api/httpmonitors")]
    public class HttpMonitorsController : Controller
    {
        private readonly IHttpMonitorService _service;

        public HttpMonitorsController(IHttpMonitorService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet, Route("{httpMonitorId:HttpMonitorId}")]
        public async Task<IActionResult> Get(HttpMonitorId httpMonitorId)
        {
            var httpMonitor = await _service.GetHttpMonitorByIdAsync(httpMonitorId);

            if (httpMonitor != null)
            {
                return Ok(httpMonitor);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateHttpMonitor command)
        {
            var httpMonitor = await _service.CreateHttpMonitorAsync(command);

            return Ok(httpMonitor);
        }

        [HttpPut, Route("{httpMonitorId:HttpMonitorId}")]
        public async Task<IActionResult> Put(HttpMonitorId httpMonitorId, [FromBody] UpdateHttpMonitor command)
        {
            command.HttpMonitorId = httpMonitorId;

            try
            {
                var httpMonitor = await _service.UpdateHttpMonitorAsync(command);

                return Ok(httpMonitor);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete, Route("{httpMonitorId:HttpMonitorId}")]
        public async Task<IActionResult> Delete(HttpMonitorId httpMonitorId)
        {
            try
            {
                await _service.DeleteHttpMonitorAsync(httpMonitorId);

                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}