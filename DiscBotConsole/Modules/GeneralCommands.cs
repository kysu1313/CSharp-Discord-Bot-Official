

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClassLibrary.Data;
using BotApi.Helpers;
using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Discord.Rest;
using Humanizer;
using BotApi.Helpers.Crypto;

namespace DiscBotConsole.Modules
{
    public class GeneralCommands : ModuleBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _service;
        private readonly CommandService _commands;
        private readonly Helper _helper;
        private DiscordSocketClient _client;
        private Reminder _reminder;

        public GeneralCommands(ApplicationDbContext context, IServiceProvider services)
        {
            _context = context;
            _service = services;
            _helper = new Helper(context, services);
            _reminder = new Reminder(context, services);
            _commands = services.GetRequiredService<CommandService>();
        }

        [Command("ping")]
        [Summary("Simple check to make sure bot is running.\n !ping")]
        public async Task PingPong()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            // build out the reply
            sb.AppendLine("🏓 Pong!");
            sb.AppendLine($"You are -> [{user.Username}]");

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("test")]
        [Summary("test")]
        public async Task Test()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            Blockchain bc = new Blockchain(_context, _service);
            var items = await bc.Testing();
            await bc.QbitTest();

            // build out the reply
            foreach (var item in items)
            {
                sb.AppendLine(item);
            }

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("remindme")]
        [RequireContext(ContextType.Guild)]
        [Summary(
            "Creates a reminder for x hours in the future.\n" +
            "Example: !remindme 3:00pm daily this is a test"
        )]
        public async Task RemindMe(
            string time,
            string increment,
            [Remainder] string task)
        {
            // !remindme 3:00pm daily "this is a test"



            var user = Context.User;
            var guild = Context.Guild;
            var exTime = DateTime.Parse(time);
            var execInc = await _helper.ParseTimeIncrement(increment);
            var repeating = await _helper.CheckReminderRepeat(execInc);

            if (task != null)
            {
                ReminderModel model = new ReminderModel()
                {
                    createdById = user.Id,
                    createdInServerId = guild.Id,
                    timeAdded = DateTime.Now,
                    executionTime = exTime,
                    executionIncrement = execInc,
                    value = task,
                    reminderId = new Guid(),
                    additionalInfo = "",
                    hasExecuted = false,
                    shouldRepeat = repeating,
                    numberOfRepeats = 1,
                    currentRepeatNumber = 0,
                    repeatIncrement = execInc,
                    endDate = false
                };

                await _helper.AddReminder(model);
                await ReplyAsync($"Reminder set for {model.executionTime.Humanize(utcDate: false)}");
            }
        }

        [Command("myreminders")]
        [RequireContext(ContextType.Guild)]
        [Summary(
            "Creates a reminder for x hours in the future.\n" +
            "Example: !remindme 3:00pm daily this is a test"
        )]
        public async Task MyReminders()
        {
            var user = Context.User;
            var guild = Context.Guild;
            var reminders = await _helper.GetUserReminders(user.Id, guild.Id);
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();

            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Reminders:";
            int count = 0;
            if (reminders.Count > 0)
            {
                reminders.Sort((x, y) => DateTime.Compare(x.executionTime, y.executionTime));
                foreach (var item in reminders)
                {
                    sb.AppendLine($"({count}). {item.executionTime}: {item.value}, {item.additionalInfo}");
                    count++;
                }
            }

            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("Help")]
        [Summary("View info about all commands and usages.\n !help")]
        public async Task Help()
        {
            List<CommandInfo> commands = _commands.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            var ch = Context.Channel;

            _client = _service.GetRequiredService<DiscordSocketClient>();
            var channel = _client.GetChannel(ch.Id) as ITextChannel;
            string embedFieldText = "";

            foreach (CommandInfo command in commands)
            {
                var nsfwCmds = new string[] { "34", "nsfw" };
                if (!channel.IsNsfw && !command.Name.Contains("34") && !command.Name.Contains("nsfw"))
                {
                    embedFieldText = command.Summary ?? "No description available\n";
                    embedBuilder.AddField(command.Name, embedFieldText);
                }
                else if (channel.IsNsfw)
                {
                    embedFieldText = command.Summary ?? "No description available\n";
                    embedBuilder.AddField(command.Name, embedFieldText);
                }
                else
                {
                    embedFieldText = command.Summary ?? "No description available\n";
                    embedBuilder.AddField("N/A", "Not avaliable in this channel");
                }
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }

        [Command("hello")]
        [Summary("Say hello")]
        public async Task HelloCommand()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            // build out the reply
            sb.AppendLine($"You are -> [{user.Username}]");
            sb.AppendLine("I must now say, World!");

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("8ball")]
        [Alias("ask")]
        [Summary("Ask a question and get a classig 8ball answer.\n !8ball <question>")]
        public async Task AskEightBall([Remainder] string args = null)
        {
            // StringBuilder to build out the reply
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            // list of possible replies
            var replies= new List<string>() {
                "It is certain.", "It is decidedly so.", "You may rely on it.", "Without a doubt.",
                   "Yes - definitely.", "As I see, yes.", "Most likely.", "Outlook good.", "Yes.",
                   "Signs point to yes.", "Reply hazy, try again.", "Ask again later.", "Better not tell you now.",
                   "Cannot predict now.", "Concentrate and ask again later.", "Don\"t count on it.", "My reply is no.",
                   "My sources say no.", "Outlook not so good.", "Very doubtful." };


            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Welcome to the 8-ball!";

            sb.AppendLine();

            if (args == null)
            {
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else
            {
                var answer = replies[new Random().Next(replies.Count - 1)];

                sb.AppendLine($"You asked: " + args);
                sb.AppendLine();
                sb.AppendLine("Response: " + answer);

                
            }

            embed.Description = sb.ToString();
            var user = Context.User;
            var guild = Context.Guild;
            await _helper.UpdateUser(guild, iusr: user);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("stats")]
        [Summary("Show stats for user or username.\n !stats <?username>")]
        public async Task ViewStats(string userName = "")
        {
            // StringBuilder to build out the reply
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            var guild = Context.Guild;
            var user = new UserExperience();

            if (userName != "")
            {
                user = await _helper.GetUserNameExperienceInServer(userName, guild.Id);
                sbTitle.AppendLine($"{user.userName}'s Stats");
            }
            else
            {
                user = await _helper.GetUserExperienceInServer(Context.User.Id, guild.Id);
                sbTitle.AppendLine($"{user.userName}'s Stats");
            }

            sb.AppendLine($"Level: {user.userLevel} / {_helper.MAX_USER_LEVEL}");
            sb.AppendLine($"Bank: ${user.bank}");
            sb.AppendLine($"Wallet: ${user.wallet}");
            sb.AppendLine($"Luck: {user.luck}");
            sb.AppendLine($"Messages: {user.messages}");
            sb.AppendLine($"Experience: {user.experience}");
            sb.AppendLine($"Emojis Sent: {user.emojiSent}");
            sb.AppendLine($"Reactions Received: {user.reactionsReceived}");


            var embed = new EmbedBuilder()
            {
                Color = Color.Purple,
                Title = sbTitle.ToString(),
                Description = sb.ToString()
            };

            await _helper.UpdateUser(guild, user: user);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("dashadd")]
        [Summary("Add element to user dashboard. NOT FULLY IMPLEMENTED")]
        public async Task AddToDash(string command = "", string value = "")
        {
            var user = Context.User;
            var guild = Context.Guild;

            var dash = _helper.GetUserDashAsync(user, guild);
        }

        [Command("dash")]
        [Summary("View user dashboard. NOT FULLY IMPLEMENTED")]
        public async Task ViewDashboard()
        {

            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            Image img = new Image();
            var guild = Context.Guild;
            var user = Context.User;
            var dash = await _helper.GetUserDashAsync(user, guild);
            var api = new Apis(_context, _service);

            foreach (var item in dash.items)
            {
                switch (item.command)
                {
                    case DashCommand.api:
                        break;
                    case DashCommand.crypto:
                        item.result = await api.CryptoApi(item.value.ToString());
                        sb.AppendLine($"{item.value}: ${item.result}");
                        break;
                    case DashCommand.image:
                        embed.ImageUrl = item.command.ToString();
                        break;
                    case DashCommand.stat:
                        sb.AppendLine($"{item.value}: {item.result}");
                        break;
                    case DashCommand.color:
                        foreach (string color in Enum.GetNames(typeof(DashColor)))
                        {
                            if (color.ToLower().Equals(dash.color.ToString().ToLower()))
                            {
                                embed.Color = new Color(((uint)dash.color));
                            }
                        }
                        break;
                }
            }

        }

    }
}