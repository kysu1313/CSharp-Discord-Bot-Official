using ClassLibrary.Helpers;
using ClassLibrary.Data;
using ClassLibrary.Models;
using Discord;
using Discord.Commands;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Helpers;

namespace DiscBotConsole.Modules
{
    public class ApiCommands : ModuleBase
    {

        private RedditBot _redditBot;
        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private readonly Apis _apis;

        public ApiCommands(ApplicationDbContext context, IServiceProvider services, RedditBot redditBot)
        {
            _context = context;
            _helper = new Helper(context, services);
            _apis = new Apis(context, services);
            _redditBot = redditBot;
        }

        [Command("joke", RunMode = RunMode.Async)]
        [Summary("Gets a random joke with punchline.")]
        [RequireContext(ContextType.Guild)]
        public async Task GetJoke()
        {
            try
            {
                string joke, punchline;
                StringBuilder sb = new StringBuilder();
                (joke, punchline) = await _apis.JokeApi();
                sb.Append(joke + "\n");
                await ReplyAsync(sb.ToString());

                await Task.Delay(3000);

                sb.Clear();
                sb.Append(punchline + "\n");
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await ReplyAsync(message:$"Aw snap! something went wrong D: {e}");
            }

        }

        [RequireNsfw]
        [Command("r34", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        [Summary("Pulls a random NSFW rule34 image. Optional tag parameter can be added for count and tags, i.e. '5 girl+boy'.")]
        public async Task Rule34([Remainder] string tags = null)
        {
            try
            {
                var user = Context.User;
                var guild = Context.Guild;

                tags = string.IsNullOrEmpty(tags) ? "" : tags;
                var tmpTags = tags.Split(" ");
                int.TryParse(tmpTags[0], out int exists);
                tags = exists == 0 ? tags : tmpTags[1];
                int count = exists == 0 ? 1 : exists > 15 ? 15 : exists;

                var results = await _apis.Rule34Api(count, tags);
                var embed = new EmbedBuilder();

                foreach (var img in results)
                {
                    embed.ImageUrl = img;
                    await ReplyAsync(null, false, embed.Build());
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(message:$"Aw snap! something went wrong D: {e}");
            }
            
        }

        [Command("reddit", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        [Summary("Get latest post from a subreddit. !reddit <subreddit name> <top / new> <number of results>")]
        public async Task RedditPopular(string? subreddit = "", string? type = "top", int? count = 1)
        {
            try
            {
                var user = Context.User;
                var guild = Context.Guild;

                string subr = subreddit == "" ? "home" : subreddit;
                int number = count < 1 || count > 20 ? 20 : (int)count;

                var result = type == "top" ? await _apis.RedditApiGetTop(_redditBot, subreddit, number) :
                                             await _apis.RedditApiGetNew(_redditBot, subreddit, number);
                var embed = new EmbedBuilder();


                if (result != null)
                {
                    if (result.posts.Count == 0)
                    {
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.AddField("Sorry no results", "Either the subreddit was not found or it is nsfw. Nsfw subreddits must be called from nsfw channels.");
                    }
                    else
                    {
                        foreach (var post in result.posts)
                        {
                            EmbedBuilder eb = new EmbedBuilder();
                            if (!post.NSFW)
                            {
                                eb.AddField(
                                    post.Title.Substring(0, Math.Min(post.Title.Length, 150)),
                                    post.Shortlink
                                );
                                eb.ImageUrl = post.Url.ToString();
                                await ReplyAsync(null, false, eb.Build());
                            }
                            else
                            {
                                eb.AddField("NSFW", "NSFW");
                                await ReplyAsync(null, false, eb.Build());
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                await ReplyAsync($"Something went wrong: {e}");
            }
        }

        [RequireNsfw]
        [Command("redditnsfw", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        [Summary("Get latest post from a subreddit. !reddit <subreddit name> <top / new> <number of results>")]
        public async Task RedditNsfw(string? subreddit = "", string? type = "top", int? count = 1)
        {
            try
            {
                var user = Context.User;
                var guild = Context.Guild;

                string subr = subreddit == "" ? "home" : subreddit;
                int number = count < 1 || count > 5 ? 5 : (int)count;

                var result = type == "top" ? await _apis.RedditApiGetTop(_redditBot, subreddit, number) :
                                             await _apis.RedditApiGetNew(_redditBot, subreddit, number);
                var embed = new EmbedBuilder();

                if (result != null)
                {
                    foreach (var post in result.posts)
                    {
                        EmbedBuilder eb = new EmbedBuilder();
                        var url = post.Url.ToString(); ;

                        if (url.Contains(".gifv"))
                        {
                            url = url.ToString().Split(".gif")[0];
                        }
                        eb.AddField(
                            post.Title.Substring(0, Math.Min(post.Title.Length, 150)),
                            url
                        );
                        eb.ImageUrl = url;
                        await ReplyAsync(null, false, eb.Build());
                    }
                }
            }
            catch(Exception e)
            {
                await ReplyAsync($"Ah, something went wrong! {e}");
            }
                

        }

    }

    
}
