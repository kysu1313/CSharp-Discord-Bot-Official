using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ClassLibrary.Data;
using BotApi.Helpers;
using DiscBotConsole;

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

            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;

        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
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

            await _commands.ExecuteAsync(context, argPos, _services);
            // execute command if one is found that matches
            //using (var scope = _services.CreateScope())
            //{
            //    await _commands.ExecuteAsync(context, argPos, scope.ServiceProvider);
            //}
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
