using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClassLibrary.Data;
using ClassLibrary.ModelDTOs;
using RedditSharp;
using RedditSharp.Things;
using ClassLibrary.Models;
using System.Collections.Generic;
using ClassLibrary.Models.HelperModels;
using System.Collections;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace BusinessLogic.Helpers
{
    public class Apis
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private static HttpClient _client; 
        private readonly RefreshTokenWebAgentPool _redditAgentPool;
        private readonly string[] imgext = { ".img", ".jpg", ".jpeg", ".png", ".tiff", ".raw" };

        public Apis(ApplicationDbContext context, IServiceProvider services) // , RefreshTokenWebAgentPool webAgentPool
        {
            _context = context; 
            _config = services.GetRequiredService<IConfiguration>();
            //_redditAgentPool = webAgentPool;
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

        private async Task<int> GetItemCount(string url)
        {
            int num;
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webresponse = (HttpWebResponse)await webrequest.GetResponseAsync();
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream());

            var result = responseStream.ReadToEnd();
            num = result.Length;
            return num;
        }

        public static async Task<IList<Rule34Post>> PostCallAPI(string url, object jsonObject)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;
                    if (response != null)
                    {
                        return await client.GetFromJsonAsync<IList<Rule34Post>>("Posts");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public async Task<List<string>> Rule34Api(int count = 1, string tags = "")
        {
            string url = "";
            var img = "";
            count = count > 15 ? 15 : count;
            Random rand = new Random();
            var url1 = $"http://www.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&tags=" + tags;
            int total = await GetItemCount(url1);
            int rint = rand.Next(0, total);
            int res = 500;

            if (tags != "")
            {
                tags += "+score:>=10";
                url = $"http://www.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&limit={res}&id={rint}&tags={tags}";
            }
            else
            {
                url = $"http://www.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&limit=1&id={rint}";
            }

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webresponse = (HttpWebResponse)await webrequest.GetResponseAsync();
            WebHeaderCollection header = webresponse.Headers;

            var encoding = ASCIIEncoding.ASCII;
            var lst = new List<string>();
            using (var reader = new System.IO.StreamReader(webresponse.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                if (responseText != "")
                {
                    var result = responseText;
                    var json = JArray.Parse(result);
                    if (!json.Children().Any())
                    {
                        img = "No good results found :(";
                    }
                    else
                    {
                        var tmpCount = 0;
                        for (int i = 0; i < count; i++)
                        {
                            int randRes = rand.Next(i+tmpCount, json.Children().Count());
                            var ingUrl = JObject.Parse(json[randRes].ToString());
                            while (ingUrl.SelectToken("file_url").ToString().Contains(".mp4")
                                || ingUrl.SelectToken("file_url").ToString().Contains(".gifv"))
                            {
                                randRes = rand.Next(i+tmpCount, json.Children().Count());
                                ingUrl = JObject.Parse(json[randRes].ToString());
                                tmpCount++;
                            }
                            lst.Add(ingUrl.SelectToken("file_url").ToString());
                        }
                        
                    }
                }
            }
            return lst;
        }

        public async Task<RedditResponse> RedditApiGetTop(RedditBot bot, string subreddit, int number)
        {
            _client = new HttpClient();

            if (!subreddit.Contains("/r/"))
            {
                subreddit = "/r/" + subreddit.Trim(new char[] { 'r', '/' });
            }

            var webAgent = new BotWebAgent(bot._username, bot._password, bot._id, bot._token, bot._endpoint);
            var reddit = new Reddit(webAgent, false);
            var subr = await reddit.GetSubredditAsync(subreddit);

            await subr.SubscribeAsync();
            var posts = await subr.GetPosts().ToListAsync();
            var postResults = new List<Post>();
            int count = 0;
            while (count < number)
            {
                if (posts.Count >= count)
                {
                    var post = posts.ElementAt(count);
                    bool added = false;
                    foreach (var ex in imgext)
                    {
                        if (post.Url.ToString().Contains(ex))
                        {
                            postResults.Add(post);
                            added = true;
                            count++;
                            break;
                        }
                    }
                    if (!added)
                    {
                        posts.Remove(post);
                    }
                }
                else
                {
                    break;
                }
            }
            var result = new RedditResponse()
            {
                subreddit = subreddit,
                posts = postResults,
                isNsfw = subr.NSFW
            };
            return result;
        }

        public async Task<RedditResponse> RedditApiGetNew(RedditBot bot, string subreddit, int number)
        {
            _client = new HttpClient();

            if (!subreddit.Contains("/r/"))
            {
                subreddit = "/r/" + subreddit.Trim(new char[] { 'r', '/' });
            }

            var webAgent = new BotWebAgent(bot._username, bot._password, bot._id, bot._token, bot._endpoint);
            var reddit = new Reddit(webAgent, false);
            var subr = await reddit.GetSubredditAsync(subreddit);

            await subr.SubscribeAsync();
            var posts = await subr.GetPosts().ToListAsync();
            var postResults = new List<Post>();
            int count = 0;
            while (count < number)
            {
                if (posts.Count >= count)
                {
                    var post = posts.ElementAt(count);
                    bool added = false;
                    foreach (var ex in imgext)
                    {
                        if (post.Url.ToString().Contains(ex))
                        {
                            postResults.Add(post);
                            added = true;
                            count++;
                            break;
                        }
                    }
                    if (!added)
                    {
                        posts.Remove(post);
                    }
                }
                else
                {
                    break;
                }
            }
            var result = new RedditResponse()
            {
                subreddit = subreddit,
                posts = postResults,
                isNsfw = subr.NSFW
            };
            return result;
        }

    }
}
