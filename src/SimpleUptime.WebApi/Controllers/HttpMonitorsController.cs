using System;
using Microsoft.AspNetCore.Mvc;

namespace SimpleUptime.WebApi.Controllers
{
    [Route("api/httpmonitors")]
    public class HttpMonitorsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet, Route("{id}")]
        public IActionResult Get(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public IActionResult Post()
        {
            throw new NotImplementedException();
        }

        [HttpPost, Route("{id}")]
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