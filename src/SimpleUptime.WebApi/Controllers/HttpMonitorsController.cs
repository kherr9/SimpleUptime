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

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> Get([ModelBinder(typeof(HttpMonitorIdModelBinder))]HttpMonitorId id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpMonitor = await _service.GetHttpMonitorByIdAsync(id.Value.ToString());

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

        [HttpPut, Route("{id}")]
        public async Task<IActionResult> Put([ModelBinder(typeof(HttpMonitorIdModelBinder))]HttpMonitorId id, [FromBody] UpdateHttpMonitor command)
        {
            if (id == null)
            {
                return NotFound();
            }

            command.HttpMonitorId = id.ToString();

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

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> Delete([ModelBinder(typeof(HttpMonitorIdModelBinder))]HttpMonitorId id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                await _service.DeleteHttpMonitorAsync(id.ToString());

                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}