using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazorise.Charts;
using ClassLibrary.Data;
using BusinessLogic.Helpers;
using ClassLibrary.Models;
using ClassLibrary.Models.ContextModels;
using ClassLibrary.Models.Utility;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using static Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace BotDash.ComponentModels
{
    public class UsageGraphModel : ComponentBase
    {
        private List<ServerModel> _servers;
        private List<UserExperience> _users;
        private Helper _helper;
        private readonly string _baseUrl = "https://localhost:5003/api/";
        
        protected override async Task OnInitializedAsync()
        {
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
            List<UserExperience> usrs = new List<UserExperience>();  

            using (var client = new HttpClient())  
            {  
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Clear();  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  

                //Sending request to find web api REST service resource GetDepartments using HttpClient  
                HttpResponseMessage res = await client.GetAsync("HelperApi/api/getusers");  
                if (res.IsSuccessStatusCode)  
                {
                    var objResponse = res.Content.ReadAsStringAsync().Result;  
                    usrs = JsonConvert.DeserializeObject<List<UserExperience>>(objResponse);
                }
                return usrs;
            }  
        }

        private async Task<List<ServerModel>> GetServers()
        {
            List<ServerModel> svrs = new List<ServerModel>();  

            using (var client = new HttpClient())  
            {  
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Clear();  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  

                //Sending request to find web api REST service resource GetDepartments using HttpClient  
                HttpResponseMessage res = await client.GetAsync("HelperApi/api/getservers");  
                if (res.IsSuccessStatusCode)  
                {
                    var objResponse = res.Content.ReadAsStringAsync().Result;  
                    svrs = JsonConvert.DeserializeObject<List<ServerModel>>(objResponse);
                }
                return svrs;
            }  
        }
    }
}