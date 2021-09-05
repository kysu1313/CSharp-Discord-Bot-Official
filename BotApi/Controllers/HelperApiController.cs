using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperApiController : ControllerBase
    {
        // GET: api/HelperApi
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/HelperApi/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/HelperApi
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/HelperApi/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/HelperApi/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
