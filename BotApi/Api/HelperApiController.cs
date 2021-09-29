using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLogic.Helpers;
using ClassLibrary.Data;
using ClassLibrary.DataContext;
using BusinessLogic.ModelDTOs;
using BusinessLogic.Models;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace BotApi.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperApiController : ControllerBase
    {

        private ApplicationDbContext _context;
        private IServiceProvider _services;
        private Helper _helper;
        
        public HelperApiController(ApplicationDbContext context, IServiceProvider services)
        {
            _helper = new Helper(context, services);
            _context = context;
            _services = services;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getservers")]
        public async Task<string> GetTotalServers()
        {
            var servers = await _helper.getAllServerModels();
            var jsonStr = JsonConvert.SerializeObject(servers);
            return jsonStr;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusers")]
        public async Task<string> GetTotalUsers()
        {
            var users = await _helper.getAllUserExperiences();
            var jsonStr = JsonConvert.SerializeObject(users);
            return jsonStr;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusersinserver{serverId}")]
        public async Task<string> GetUsersInServer(string serverId)
        {
            var servId = ulong.TryParse(serverId, out ulong sid) == false ? 0 : sid;
            var users = await _helper.getAllUserInServer(servId);
            var jsonStr = JsonConvert.SerializeObject(users);
            return jsonStr;
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getcommandsinserver{string}")]
        public async Task<string> GetCommandsInServers(string userId)
        {
            var servers = await _helper.getAllServerModels();
            var jsonStr = JsonConvert.SerializeObject(servers);
            return jsonStr;
        }

        // GET: api/HelperApi/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<string> Get(int id)
        {
            return "value";
        }

        // POST: api/HelperApi
        [HttpPost]
        [Route("api/postuserid")]
        public async Task PostUserId([FromBody] string discId)
        {
            await using (var dto = new UserModelDTO(_context))
            {
                
            }
        }

        // PUT: api/HelperApi/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/HelperApi/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
        }
    }
}
