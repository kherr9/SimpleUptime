using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleUptime.Application.Services;

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
        public async Task<IActionResult> Get(string id)
        {
            var httpMonitor = await _service.GetHttpMonitorByIdAsync(id);

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
        public IActionResult Put(string id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete, Route("{id}")]
        public IActionResult Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}