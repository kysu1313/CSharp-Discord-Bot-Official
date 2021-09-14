using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ClassLibrary.Data;
using ClassLibrary.Helpers;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using DiscBotConsole;
using DiscBotConsole.Modules;
using k8s.KubeConfigModels;

namespace ClassLibrary.Models.GeneralCommands
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandParser _parser;

        public CommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _context = services.GetRequiredService<ApplicationDbContext>();
            _services = services;
            _parser = new CommandParser(_context, services);
            // take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;
            // take action when we receive a message
            _client.MessageReceived += MessageReceivedAsync;

        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            
            List<UserExperience> users;
            List<ServerModel> currSevers;

            await using (var dto = new ServerModelDTO(_context)) 
            { currSevers = (await dto.GetAllServers()).ToList(); }
            await using (var dto = new UserExperienceDTO(_context, _services)) 
            { users = (await dto.GetAllUserExperiences()).ToList(); }
            var distinctLst = users.Select(x => x.serverId)
                .Distinct()
                .ToList()
                .Except(currSevers.Select(x => x.serverId))
                .ToList();

            if (distinctLst.Count > 0)
            {
                await using (var dto = new ServerModelDTO(_context))
                {
                    foreach (var svr in distinctLst)
                    {
                        var svrName = _client.GetGuild(svr).Name;
                        dto.AddServer(new ServerModel()
                        {
                            serverId = svr,
                            serverName = string.IsNullOrEmpty(svrName) ? "None" : svrName,
                        });
                    }
                }
            }
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        [RequireContext(ContextType.Guild)]
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            // This is a hack to get the current guild ID
            var guilds = _client.Guilds.ToList();
            SocketGuildChannel server = null;
            SocketGuild currGuild = null;
            bool runCommand = true;
            
            foreach (var g in guilds)
            {
                server = g.Channels.FirstOrDefault(x => x.Id == message.Channel.Id);
                currGuild = g;
                if (server != null)
                    break;
            }
            
            // Check for disabled commands. This needs to run for every command.
            await using (var dto = new CommandModelDTO(_context, _services))
            {
                var allCmds = new List<string>();
                allCmds.AddRange(_commands.Commands.ToList().Select(x => x.Name));
                var currCmds = (await dto.GetCommands(currGuild.Id).ConfigureAwait(false));
                var existingCmds = currCmds.Select(x => x.commandName);
                var tmpLst = allCmds.Except(existingCmds).ToList();
                if (tmpLst.Count > 0)
                {
                    await dto.AddCommands(tmpLst, message.Author, currGuild);
                }

                var tmpCmd = currCmds.FirstOrDefault(x => 
                    x.commandName == message.Content.ToString().Substring(1));
                runCommand = tmpCmd != null ? tmpCmd.enabled : true;
            }
            

            // sets the argument position away from the prefix
            var argPos = 0;

            // get prefix from the configuration file
            char prefix = Char.Parse(_config["Prefix"]);

            // TODO: Parse message here.
            var context = new SocketCommandContext(_client, message);
            await _parser.ParseMessage(rawMessage, context);

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)) || message.Author.IsBot)
            {
                return;
            }

            if (runCommand)
            {
                await _commands.ExecuteAsync(context, argPos, _services);    
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {

            //if (context is ScopedCommandContext scopedCommandContext)
            //{
            //    scopedCommandContext.ServiceScope.Dispose();
            //}

            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                System.Console.WriteLine($"Command failed to execute for [{context.User.Username}] <-> [{command.Value.ToString()}]!");
                return;
            }

            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                string cmd = command.Value.Name;
                System.Console.WriteLine($"Command [{cmd}] executed for -> [{context.User.Username}]");
                return;
            }


            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync(
                $"Sorry, ... something went wrong -> {result.Error} \n" +
                $"Cause: {result.ErrorReason}" +
                $"!");
        }
    
        private async Task<IEnumerable<string>> GetMethods(Type type)
        {
            var list = new List<string>();
            foreach (var method in type.GetMethods())
            {
                if (method.IsPublic)
                    list.Add(method.Name);
            }
            return list;
        }
    }
    
    



    //public class ScopedCommandContext : CommandContext
    //{
    //    public IServiceScope ServiceScope { get; }

    //    public ScopedCommandContext(
    //        IServiceScope serviceScope,
    //        global::Discord.IDiscordClient client,
    //        global::Discord.IUserMessage msg) : base(client, msg)
    //    {
    //        ServiceScope = serviceScope;
    //    }
    //}
}
