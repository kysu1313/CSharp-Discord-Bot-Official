using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotDash.Logic;
using BusinessLogic.ModelDTOs;
using ClassLibrary.Data;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Radzen;

namespace BotDash.Models.PageModels
{
    public class ControlPanelModel : ComponentBase
    {

        protected bool _isLoggedIn = false;
        protected bool _hasLinkedAccount = false;
        protected Dictionary<string, ulong> _severNames;
        protected Dictionary<string, int> _commandNames;
        protected List<ServerModel> _servers;
        protected List<CommandModel> _commands;
        protected ServerModel _selectedServer;
        protected List<CommandModel> _selectedCommands;
        private AuthenticationState _currAuth;
        private UserModel _currUser;
        private string _baseUrl;
        [Inject] protected ApplicationDbContext Context { get; set; }
        [Inject] protected IServiceProvider Services { get; set; }
        [Inject] private IConfiguration config { get; set; }
        [Inject] protected TooltipService tooltipService { get; set; }
        
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        
        public ControlPanelModel() { }
        
        protected void ShowTooltip(ElementReference elementReference, TooltipOptions options = null) => 
            tooltipService.Open(elementReference, "If nothing appears, click the 'key' field.", options);

        protected override async Task OnInitializedAsync()
        {
            _baseUrl = config.GetValue<string>("ApiUrl");
            _currAuth = await AuthenticationStateTask.ConfigureAwait(false);
            _selectedServer = new ServerModel();
            InitAndClearVars();
            
            if (_currAuth.User.Identity != null && _currAuth.User != null && _currAuth.User.Identity.IsAuthenticated)
            {
                _isLoggedIn = true;
                
                var name = _currAuth.User.Identity.Name;
                var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getuserfromusername", name);
                var userModel = JsonConvert.DeserializeObject<UserModel>(apiResp);
                // var userModel = await dto.GetUser(name, null);
                if (userModel != null && userModel.hasLinkedAccount)
                {
                    
                    _hasLinkedAccount = true;
                }
                    
                _currUser = userModel;

                if (_servers != null && _servers.Count > 0)
                {
                    _selectedServer = _servers.First();
                    await GetServerCommands(_selectedServer.serverId);
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
                await GetServerCommands(_selectedServer.serverId);
                _selectedCommands = _commands;
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
            InitAndClearVars();
                
            // Get user model
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getuserfromid", _currUser.userId.ToString());
            var user = JsonConvert.DeserializeObject<UserModel>(apiResp);

            if (user != null && _hasLinkedAccount)
            {
                // From userId get all servers owned by user
                apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getusersservers", user.discordId);
                _servers = JsonConvert.DeserializeObject<List<ServerModel>>(apiResp);
                    
                if (_servers is { Count: > 0 })
                {
                    foreach (var s in _servers)
                    {
                        _severNames.Add(s.serverName, s.serverId);
                    }
                }
            }
            await InvokeAsync(() => { StateHasChanged(); });
        } 

        protected async Task GetServerCommands(ulong serverId)
        {
            _isLoggedIn = true;
            InitAndClearVars();
            
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getcommandsinserver", serverId.ToString());
            _commands = JsonConvert.DeserializeObject<List<CommandModel>>(apiResp);
            
            if (_commands is { Count: > 0 })
            {
                foreach (var c in _commands)
                {
                    if (_commandNames != null && c.commandName != null) 
                        _commandNames.Add(c.commandName, c.commandId);
                }                
            }
            await InvokeAsync(() => { StateHasChanged(); });
        }

        private void InitAndClearVars()
        {
            if (_commands == null) _commands = new List<CommandModel>();
            if (_commandNames == null) _commandNames = new Dictionary<string, int>();
            if (_severNames == null) _severNames = new Dictionary<string, ulong>();
            if (_servers == null) _servers = new List<ServerModel>(); 
            
            if (_commands != null) _commands.Clear();
            if (_commandNames != null) _commandNames.Clear();
            if (_severNames != null) _severNames.Clear();
            if (_servers != null) _servers.Clear(); 
        }
    }
}