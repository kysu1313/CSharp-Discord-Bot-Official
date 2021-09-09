using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazorise.Charts;
using ClassLibrary.Data;
using ClassLibrary.Helpers;
using ClassLibrary.Models;
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
        public LineChart<double> _userLineChart;
        public LineChart<double> _serverLineChart;
        public int _userCount;
        public int _serverCount;
        private readonly string _baseUrl = "https://localhost:5003/api/";
        
        protected override async Task OnInitializedAsync()
        {
            await InitializeVars();
        }

        private async Task InitializeVars()
        {
            // var serverStream = new HttpRequestMessage(HttpMethod.Get, "HelperApi/api/getservers");
            //
            // var userStream = new HttpRequestMessage(HttpMethod.Get, "HelperApi/api/getusers");
            //
            // _servers = serverStream.Content == null ? new List<ServerModel>() : await JsonSerializer.DeserializeAsync
            //     <List<ServerModel>>(await serverStream.Content.ReadAsStreamAsync());
            // _users = userStream.Content == null ? new List<UserExperience>() : await JsonSerializer.DeserializeAsync
            //     <List<UserExperience>>(await userStream.Content.ReadAsStreamAsync());
            _users = await GetUsers();
            _userLineChart = new LineChart<double>();
            _serverLineChart = new LineChart<double>();
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

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await HandleRedraw();
            }
        }

        public async Task HandleRedraw()
        {
            
            await InitializeVars();
            await _userLineChart.Clear();
            await _userLineChart.AddLabelsDatasetsAndUpdate( _labels, GetUserDataset() );
            await _serverLineChart.Clear();
            await _serverLineChart.AddLabelsDatasetsAndUpdate( _labels, GetUserDataset() );
        }

        LineChartDataset<double> GetUserDataset()
        {
            return new LineChartDataset<double>
            {
                Label = "# of randoms",
                Data = ParseUserData(_users),
                BackgroundColor = _backgroundColors,
                BorderColor = _borderColors,
                Fill = true,
                PointRadius = 2,
                BorderDash = new List<int> { }
            };
        }

        string[] _labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        List<string> _backgroundColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
        List<string> _borderColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

        private List<double> ParseUserData(List<UserExperience> users)
        {
            var list = new List<double>();
            if (users.Count > 0)
                users.Sort((x, y) => DateTime.Compare(x.dateUpdated, y.dateUpdated));
            
            foreach (var user in users)
            {
                var date = user.dateUpdated;
                double count = 0;
                foreach (var u in users)
                {
                    if (u.dateUpdated.Day.Equals(date.Day))
                    {
                        count++;
                    }
                    list.Add(count);
                }
            }

            return list;
        }
        List<double> RandomizeData()
        {
            var r = new Random( DateTime.Now.Millisecond );

            return new List<double> { r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble() };
        }
    }
}