using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BotDash.Models.PageModels
{
    public class ControlPanelModel : ComponentBase
    {

        protected bool _isLoggedIn = false;
        protected bool _hasLinkedAccount = false;
        protected List<ServerModel> _servers;
        protected List<CommandModel> _commands;
        protected ServerModel _selectedServer;
        private AuthenticationState _currAuth;
        private UserModel _currUser;
        [Inject] protected ApplicationDbContext Context { get; set; }
        [Inject] protected IServiceProvider Services { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        
        public ControlPanelModel()
        {
            
        }

        protected override async Task OnInitializedAsync()
        {
            _currAuth = await AuthenticationStateTask.ConfigureAwait(false);
            _selectedServer = new ServerModel();
            await GetServers();
            
            if (_currAuth.User.Identity != null && _currAuth.User != null && _currAuth.User.Identity.IsAuthenticated)
            {
                _isLoggedIn = true;
                using (var dto = new UserModelDTO(Context))
                {
                    var name = _currAuth.User.Identity.Name;
                    var userModel = await dto.GetUser(name);
                    if (userModel.hasLinkedAccount)
                    {
                        _hasLinkedAccount = true;
                    }
                    
                    _currUser = userModel;
                }

                if (_servers != null && _servers.Count > 0)
                {
                    _selectedServer = _servers.First();
                    await GetServerCommands(_selectedServer.serverId);
                }
            }
            base.OnInitialized();
        }

        protected void OnDdChange(object servId, string val)
        {
            _selectedServer = _servers.FirstOrDefault(x => 
                x.serverId == (ulong)servId);
        }

        protected async Task GetServers()
        {
            _isLoggedIn = true;
            await using (var dto = new ServerModelDTO(Context))
            {
                var svrs = await dto.GetAllServers();
                _servers = svrs.FindAll(x => x.botAdmin == _currUser);
            }
        } 

        protected async Task GetServerCommands(ulong serverId)
        {
            _isLoggedIn = true;
            using (var dto = new CommandModelDTO(Context, Services))
            {
                _commands = await dto.GetCommands(serverId).ConfigureAwait(false) as List<CommandModel>;
                
            }
        } 
    }
}