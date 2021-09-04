using DiscBotConsole.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DiscBotConsole.Models;
using System.Xml.Linq;

namespace DiscBotConsole.Helpers
{
    public class Apis
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private static HttpClient _client;

        public Apis(ApplicationDbContext context, IServiceProvider services)
        {
            _context = context; 
            _config = services.GetRequiredService<IConfiguration>();
        }

        private async Task<JObject> ApiBuilder(string url)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse webresponse = (HttpWebResponse)await webrequest.GetResponseAsync();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            var json = JObject.Parse(responseStream.ReadToEnd());
            webresponse.Close();
            return json;
        }

        private async Task UpdateCryptoPrices(string name, float value)
        {
            var cryptoDto = new CryptoModelDTO(_context);
            await cryptoDto.UpdatePrices(name, value);
        }

        public async Task<(string, string)> JokeApi()
        {
            _client = new HttpClient();
            string setup = "";
            string punchline = "";
            string url = "https://official-joke-api.appspot.com/random_joke";

            JObject json = await ApiBuilder(url);
            setup = json.GetValue("setup").ToString();
            punchline = json.GetValue("punchline").ToString();
            return (setup, punchline);
        }

        public async Task<string> CryptoApi(string coin)
        {
            _client = new HttpClient();
            string result = "";
            string apiKey = _config["CryptoCompareKey"];
            string url = 
                "https://min-api.cryptocompare.com/data/price?fsym=" 
                + coin 
                + "&tsyms=USD"
                + $"&api_key={apiKey}";

            JObject json = await ApiBuilder(url);
            result = json.First.ToString();
            var items = result.Split(":")[1];
            //items = items.Remove(items.Length-1);
            float value = (float)(float.TryParse(items, out float val) == false ? 0.0 : val);
            await UpdateCryptoPrices(coin, value);

            return result;
        }

        public async Task<string> Rule34Api(string[] tags = null)
        {
            string url = "";
            Random rand = new Random();
            int rint = rand.Next(0, 10000);
            int res = rand.Next(0, 400);
            if (tags != null)
            {
                string tgs = String.Join(",", tags);
                tgs.Remove(tgs.Length - 1, 1);
                tgs += "+score:>=10";
                url = $"http://www.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&limit={res}&id={rint}&tags={tgs}";
            }
            else
            {
                url = $"http://www.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&limit=1&id={rint}";
            }

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse webresponse = (HttpWebResponse)await webrequest.GetResponseAsync();
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream());

            var result = responseStream.ReadToEnd();
            var json = JArray.Parse(result);
            int randRes = rand.Next(0, json.Children().Count());
            var ingUrl = JObject.Parse(json[randRes].ToString());
            var img = ingUrl.SelectToken("file_url");

            return img.ToString();
        }

    }
}
