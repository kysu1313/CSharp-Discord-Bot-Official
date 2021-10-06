#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;
using Newtonsoft.Json;

namespace BotDash.Logic
{
    public static class ApiHelper
    {
        // private static readonly string _baseUrl = "https://localhost:5003/api/";

        public static async Task<string> CallApi(string apiUrl, string apiPath, string? id)
        {
            using var client = new HttpClient();
            //Passing service base url  
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Clear();  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  
            
            //Sending request to find web api REST service resource using HttpClient  
            if (id != null)
            {
                apiPath += $"{id}";
            }
            HttpResponseMessage res = await client.GetAsync(apiPath);  
                
            if (res.IsSuccessStatusCode)  
            {
                var objResponse = await res.Content.ReadAsStringAsync();
                return objResponse;
            }
            else
            {
                return string.Empty;
            }
        }
        
    }
}