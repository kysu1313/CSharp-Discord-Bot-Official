using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotDash.Logic;
using BusinessLogic.ModelDTOs;
using ClassLibrary.Data;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using ClassLibrary.Models.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BotDash.Models.PageModels
{
    public class UserPanelModel : ComponentBase
    {

        protected bool _isLoggedIn = false;
        protected bool _hasLinkedAccount = false;
        protected Dictionary<string, ulong> _severNames;
        protected Dictionary<string, int> _commandNames;
        protected List<ServerModel> _servers;
        protected List<CommandModel> _commands;
        protected ServerModel _selectedServer;
        protected List<UserModel> _selectedUsers;
        private AuthenticationState _currAuth;
        private UserModel _currUser;
        private string _baseUrl;
        [Inject] protected ApplicationDbContext Context { get; set; }
        [Inject] protected IServiceProvider Services { get; set; }
        [Inject] private IConfiguration config { get; set; }
        
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        
        public UserPanelModel()
        {
            
        }

        protected override async Task OnInitializedAsync()
        {
            
            _baseUrl = config.GetValue<string>("ApiUrl");
            _currAuth = await AuthenticationStateTask.ConfigureAwait(false);
            _selectedServer = new ServerModel();
            _severNames = new Dictionary<string, ulong>();
            _commandNames = new Dictionary<string, int>();
            _selectedUsers = new List<UserModel>();
            

            
            if (_currAuth.User.Identity != null && _currAuth.User != null && _currAuth.User.Identity.IsAuthenticated)
            {
                _isLoggedIn = true;
                // using (var dto = new UserModelDTO(Context))
                // {
                //     var name = _currAuth.User.Identity.Name;
                //     var userModel = await dto.GetUser(name, null);
                //     if (userModel.hasLinkedAccount)
                //     {
                //         _hasLinkedAccount = true;
                //     }
                //     
                //     _currUser = userModel;
                // }

                if (_servers != null && _servers.Count > 0)
                {
                    _selectedServer = _servers.First();
                    // await GetServerCommands(_selectedServer.serverId);
                }
                await GetServers();
            }
            base.OnInitialized();
        }

        protected async Task OnDdChange(object servId, string val)
        {
            _selectedServer = _servers.FirstOrDefault(x => 
                x.serverId == (ulong)servId);
            if (_selectedServer is not null)
            {
                // await GetServerCommands(_selectedServer.serverId);
                // _selectedCommands = _commands;
            }
        }

        /// <summary>
        /// Refresh the current servers and commands.
        /// </summary>
        protected async Task RefreshClick()
        {
            await GetServers();
        }

        protected async Task OnSwitchChange(object enabled, object cmd, string val)
        {
            var command = (CommandModel)cmd;
            var userId = _currUser.userId.ToString();
            var commandId = command.commandId;
            var serverId = _selectedServer.serverName;
            var apiResp = await ApiHelper.CallApi(
                _baseUrl, 
                $"HelperApi/api/setcommandstatus/{userId}/{commandId}/{serverId}/{enabled}", 
                _currUser.userId.ToString());
        }

        protected async Task GetServers()
        {
            _isLoggedIn = true;
            if (_commands != null) _commands.Clear();
            if (_commandNames != null) _commandNames.Clear();
            if (_severNames != null) _severNames.Clear();
                
            // Get user model
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getuser", _currUser.userId.ToString());
            var user = JsonConvert.DeserializeObject<UserModel>(apiResp);

            if (user != null)
            {
                // From userId get all servers owned by user
                apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getusersservers", user.userId.ToString());
                var svrs = JsonConvert.DeserializeObject<List<ServerModel>>(apiResp);
                    
                if (svrs is { Count: > 0 })
                {
                    foreach (var s in _servers)
                    {
                        if (_severNames != null) _severNames.Add(s.serverName, s.serverId);
                    }
                }
            }
            
            // await using (var dto = new ServerModelDTO(Context))
            // {
            //     if (_commands != null) _commands.Clear();
            //     if (_commandNames != null) _commandNames.Clear();
            //     if (_severNames != null) _severNames.Clear();
            //     
            //     // Get user model
            //     var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getuser", _currUser.userId.ToString());
            //     var user = JsonConvert.DeserializeObject<UserModel>(apiResp);
            //
            //     if (user != null)
            //     {
            //         // From userId get all servers owned by user
            //         apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getusersservers", user.userId.ToString());
            //         var svrs = JsonConvert.DeserializeObject<List<ServerModel>>(apiResp);
            //         
            //         if (svrs is { Count: > 0 })
            //         {
            //             foreach (var s in _servers)
            //             {
            //                 if (_severNames != null) _severNames.Add(s.serverName, s.serverId);
            //             }
            //         }
            //     }
            // }
        } 

        protected async Task GetUsers(ulong serverId)
        {
            _isLoggedIn = true;
            
            if (_commands != null) _commands.Clear();
            if (_commandNames != null) _commandNames.Clear();
            if (_severNames != null) _severNames.Clear();
            
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getcommandsinserver", serverId.ToString());
            _commands = JsonConvert.DeserializeObject<List<CommandModel>>(apiResp);
            
            if (_commands is { Count: > 0 })
            {
                foreach (var c in _commands)
                {
                    if (_commandNames != null) 
                        _commandNames.Add(c.commandName, c.commandId);
                }                
            }
        } 
    }
}