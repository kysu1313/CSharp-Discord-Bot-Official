using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using ClassLibrary.Models.GeneralCommands;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;
using ClassLibrary.Data;
using ClassLibrary.Helpers;
using ClassLibrary.Models;
using ClassLibrary.Models.Utility;

namespace DiscBotConsole
{
    public class Program
    {
        // setup our fields we assign later
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private static ApplicationDbContext _context;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }



        public Program()
        {
            // Replace connection string with below for testing
            // Server=(localdb)\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Tru
            // Test DB name: 20210910022534_testDb
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) //   <---- UNCOMMENT FOR MIGRATIONS
                // .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile(path: "appsettings.json")
                .AddUserSecrets<Program>();

            // build the configuration and assign to _config          
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                //_context = new ApplicationDbContext(services);
                // setup logging and the ready event
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                //_client.Guilds +=
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // this is where we get the Token value from the configuration file, and start the bot
                var tok = _config["Token"];
                await client.LoginAsync(TokenType.Bot, tok);
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync to start
                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                //await _client.DownloadUsersAsync(_client.Guilds);
                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        // this method handles the ServiceCollection creation/configuration
        private ServiceProvider ConfigureServices()
        {
            RedditBot redditBot = new RedditBot(
                _config["RedditTestId"],
                _config["RedditTestUsername"],
                _config["RedditTestPassword"],
                _config["RedditTestSecret"],
                _config["RedditTestEndpoint"]
                );

            // returns a ServiceProvider that is used later to call for those services
            // return new ServiceCollectionExtension.AddServices()
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddTransient<CommandContext>()
                .AddSingleton<CommandHandler>()
                .AddScoped<AudioService>()
                .AddDbContext<ApplicationDbContext>()
                .AddSingleton(redditBot)
                .BuildServiceProvider();
        }
    }

}
