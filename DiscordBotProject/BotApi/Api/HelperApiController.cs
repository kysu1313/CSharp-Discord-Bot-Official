using System;
using System.Collections.Generic;
using System.Linq;
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
        [Route("api/getuserfromusername{userName}")]
        public async Task<ActionResult> GetUserFromUsername(string userName)
        {
            try
            {
                var user = await _helper.getUser(userName);
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        // GET: api/HelperApi/getuser{id}
        [HttpGet]
        [Route("api/getuserfromid{userId}")]
        public async Task<ActionResult> GetUserFromId(string userId)
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
        [Route("api/getcommandsinserver{userId}")]
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

        // GET: api/HelperApi
        [HttpGet]
        [Route("api/gettotalcommands")]
        public async Task<ActionResult> GetTotalCommands()
        {
            try
            {
                var list = new List<CommandModel>();
                var servers = await _helper.getAllServerModels();
                
                if (servers.Count <= 0)
                {
                    return NotFound();
                }

                await using (var dto = new CommandModelDTO(_context, _services))
                {
                    foreach (var svr in servers)
                    {
                        list.AddRange(await dto.GetCommands(svr.serverId));
                    }
                }
                
                return Ok(list);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/HelperApi
        [HttpGet]
        [Route("api/gettotalusercommands{userId}")]
        public async Task<ActionResult> GetTotalUserCommands(string userId)
        {
            try
            {
                var list = new List<CommandModel>();
                var uid = ulong.TryParse(userId, out var id) == false ? 0 : id;
                var servers = (await _helper.getAllServerModels())
                    .FindAll(x => x.userIdent == uid);
                
                if (servers.Count <= 0)
                {
                    return NotFound();
                }

                await using (var dto = new CommandModelDTO(_context, _services))
                {
                    foreach (var svr in servers)
                    {
                        list.AddRange(await dto.GetCommands(svr.serverId));
                    }
                }
                
                return Ok(list);
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
        [Route("api/setcommandstatus/{userId}/{commandId}/{serverId}/{enabled}")]
        public async Task SetCommandStatus([FromBody] string userId, string serverCid, string serverId, string enabled)
        {
            await using (var dto = new CommandModelDTO(_context, _services))
            {
                var uid = ulong.TryParse(userId, out var uuid) == false ? 0 : uuid;
                var cid = int.TryParse(serverCid, out var ccid) == false ? 0 : ccid;
                var sid = ulong.TryParse(serverCid, out var ssid) == false ? 0 : ssid;
                var enbld = bool.TryParse(enabled, out var enbl) != false && enbl;
                var command = await dto.GetServerCommands(sid);
                if (command != null)
                {
                    var cmd = await dto.GetCommand(cid);
                    if (cmd != null)
                    {
                        await dto.UpdateCommandStatus(cmd.commandName, enbl, sid, uid);
                    }
                }
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
