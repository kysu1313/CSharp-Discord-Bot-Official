using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BotDash.Logic;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace BotDash.Models.ComponentModels
{
    public class UsageGraphModel : ComponentBase
    {
        [Inject] private IConfiguration config { get; set; }
        private List<ServerModel> _servers;
        private List<UserExperience> _users;
        // private Helper _helper;
        private string _baseUrl;
        
        protected override async Task OnInitializedAsync()
        {
            _baseUrl = config.GetValue<string>("ApiUrl");
            await InitializeVars();
        }

        private async Task InitializeVars()
        {
            _users = await GetUsers();
            _servers = await GetServers();
            
        }

        public string GetUserCount()
        {
            return _users == null ? "No users currently" : $"Total Users: {_users.Count}";
        }

        public string GetServerCount()
        {
            return _servers == null ? "No servers currently" : $"Total Servers: {_servers.Count}";
        }

        private async Task<List<UserExperience>> GetUsers()
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getusers", null);
            var usrs = JsonConvert.DeserializeObject<List<UserExperience>>(apiResp);
            return usrs;
        }

        private async Task<List<ServerModel>> GetServers()
        {
            var apiResp = await ApiHelper.CallApi(_baseUrl, "HelperApi/api/getservers", null);
            var svrs = JsonConvert.DeserializeObject<List<ServerModel>>(apiResp);
            return svrs; 
        }
    }
}