using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Helpers;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Mvc;

namespace BotApi.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperApiController : ControllerBase
    {

        private ApplicationDbContext _context;
        private Helper _helper;
        
        public HelperApiController(ApplicationDbContext _context, IServiceProvider services)
        {
            _helper = new Helper(_context, services);
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getservers")]
        public async Task<List<ServerModel>> GetTotalServers()
        {
            var servers = await _helper.getAllServerModels();
            return servers;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusers")]
        public async Task<List<UserExperience>> GetTotalUsers()
        {
            var users = await _helper.getAllUserExperiences();
            return users;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusersinserver{string}")]
        public async Task<List<UserExperience>> GetUsersInServer(string userId)
        {
            var users = await _helper.getAllUserExperiences();
            return users;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getcommandsinserver{string}")]
        public async Task<List<ServerModel>> GetCommandsInServers(string userId)
        {
            var servers = await _helper.getAllServerModels();
            return servers;
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
