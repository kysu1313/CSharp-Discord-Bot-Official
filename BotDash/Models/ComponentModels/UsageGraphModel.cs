using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BotDash.Logic;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace BotDash.Models.ComponentModels
{
    public class UsageGraphModel : ComponentBase
    {
        [Inject] private IConfiguration config { get; set; }
        protected List<ServerModel> _servers;
        protected List<UserExperience> _users;
        protected List<CommandModel> _commands;
        protected List<ServerModel> _totalServers;
        protected List<UserExperience> _totalUsers;
        protected List<CommandModel> _totalCommands;
        protected List<ServerModel> _myServers;
        protected List<UserExperience> _myUsers;
        protected bool _isLoggedIn = false;
        private AuthenticationState _currAuth;
        protected int _yAxis;
        // private Helper _helper;
        private string _baseUrl;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            _baseUrl = config.GetValue<string>("ApiUrl");
            _currAuth = await AuthenticationStateTask.ConfigureAwait(false);
            await InitializeVars();
        }

        private async Task InitializeVars()
        {
            // if (_currAuth.User is { Identity: { IsAuthenticated: true } })
            // {
            //     _isLoggedIn = true;
            //     var name = _currAuth.User.Identity.Name;
            //     var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getuserfromusername", name);
            //     var userModel = JsonConvert.DeserializeObject<UserModel>(apiResp);
            //     // var userModel = await dto.GetUser(name, null);
            //     if (userModel != null && userModel.hasLinkedAccount)
            //     {
            //         _servers = await GetPersonalServers(userModel);
            //         _commands = await GetPersonalCommands(userModel);
            //         GetYAxis(_servers, _commands);
            //     }
            // }
            // _totalUsers = await GetUsers();
            // _totalServers = await GetServers();
            // _totalCommands = await GetCommands();
            // GetYAxis(_totalServers, _totalCommands);
        }

        private void GetYAxis(List<ServerModel> svrs, List<CommandModel> cmds)
        {
            _yAxis = cmds.Count > svrs.Count ? cmds.Count : svrs.Count;
        }

        public string GetUserCount()
        {
            return _users == null ? "No users currently" : $"Total Users: {_users.Count}";
        }

        public string GetServerCount()
        {
            return _servers == null ? "No servers currently" : $"Total Servers: {_servers.Count}";
        }

        private async Task<List<ServerModel>> GetPersonalServers(UserModel usr)
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getusersservers", usr.discordId);
            var svrs = JsonConvert.DeserializeObject<List<ServerModel>>(apiResp);
            return svrs; 
        }

        private async Task<List<CommandModel>> GetPersonalCommands(UserModel usr)
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/gettotalusercommands", usr.discordId);
            var svrs = JsonConvert.DeserializeObject<List<CommandModel>>(apiResp);
            return svrs; 
        }

        private async Task<List<UserExperience>> GetUsers()
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getusers", null);
            var usrs = JsonConvert.DeserializeObject<List<UserExperience>>(apiResp);
            return usrs;
        }

        private async Task<List<ServerModel>> GetServers()
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/gettotalcommands", null);
            var svrs = JsonConvert.DeserializeObject<List<ServerModel>>(apiResp);
            return svrs; 
        }

        private async Task<List<CommandModel>> GetCommands()
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getservers", null);
            var svrs = JsonConvert.DeserializeObject<List<CommandModel>>(apiResp);
            return svrs; 
        }
    }
}