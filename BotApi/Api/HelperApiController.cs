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
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult> GetTotalServers()
        {
            try
            {
                var servers = await _helper.getAllServerModels();
                if (servers.Count <= 0)
                {
                    return NotFound();
                }
                return Ok(servers);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusers")]
        public async Task<ActionResult> GetTotalUsers()
        {
            try
            {
                List<UserExperience> users = await _helper.getAllUserExperiences();
                // var jsonStr = JsonConvert.SerializeObject(users);
                // return jsonStr;
                if (users.Count <= 0)
                {
                    return NotFound();
                }
                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        // GET: api/HelperApi/getuser{id}
        [HttpGet]
        [Route("api/getuser{userId}")]
        public async Task<ActionResult> GetUser(string userId)
        {
            try
            {
                var usrId = ulong.TryParse(userId, out ulong uid) == false ? 0 : uid;
                var user = await _helper.getUser(usrId);
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusersservers{userId}")]
        public async Task<ActionResult> GetUsersServers(string userId)
        {
            try
            {
                var usrId = ulong.TryParse(userId, out ulong uid) == false ? 0 : uid;
                var svrs = await _helper.GetUsersServers(usrId);
                if (svrs.Count <= 0)
                {
                    return NotFound();
                }
                return Ok(svrs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getusersinserver{serverId}")]
        public async Task<ActionResult> GetUsersInServer(string serverId)
        {
            try
            {
                var servId = ulong.TryParse(serverId, out ulong sid) == false ? 0 : sid;
                var users = await _helper.getAllUserInServer(servId);
                if (users.Count <= 0)
                {
                    return NotFound();
                }
                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/HelperApi
        [HttpGet]
        [Route("api/getcommandsinserver{string}")]
        public async Task<ActionResult> GetCommandsInServers(string userId)
        {
            try
            {
                var servers = await _helper.getAllServerModels();
                if (servers.Count <= 0)
                {
                    return NotFound();
                }
                return Ok(servers);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/HelperApi/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<string> Get(int id)
        {
            return "value";
        }

        // POST: api/HelperApi
        [HttpPost]
        [Route("api/postenablecommand/{userId}/{commandId}/{enabled}")]
        public async Task PostUserId([FromBody] string userId, string commandId, bool enabled)
        {
            await using (var dto = new CommandModelDTO(_context, _services))
            {
                var uid = ulong.TryParse(userId, out var uuid) == false ? 0 : uuid;
                var cid = ulong.TryParse(commandId, out var id) == false ? 0 : id;
                var command = dto.GetCommand(cid);
                // var commands = dto.UpdateCommandStatus(
                //     command.commandName, 
                //     (bool)enabled, 
                //     _selectedServer.serverId, 
                //     _currUser.userId);
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
