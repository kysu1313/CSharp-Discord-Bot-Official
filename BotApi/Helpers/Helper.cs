using BotApi.ModelDTOs;
using ClassLibrary.Data;
using ClassLibrary.DataContext;
using ClassLibrary.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BotApi.Helpers
{
    public class Helper : ModuleBase
    {
        public int STARTING_MONEY = 500;
        public int MAX_USER_LEVEL = 100;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        //private readonly ICommandContext __commandContext;
        private readonly IServiceProvider _service;
        private static ICommandContext _staticCommandContext;
        private static IServiceProvider _staticService;
        private readonly UserExperienceDTO _userExperienceDTO;
        private readonly UserModelDTO _userModelDTO;
        private readonly ServerModelDTO _serverModelDTO;
        private ReminderModelDTO _reminderModelDTO;

        public Helper(ApplicationDbContext context, IServiceProvider services, ICommandContext commandContext = null)
        {
            _context = context;
            _service = services;
            _config = services.GetRequiredService<IConfiguration>();
            _staticCommandContext = commandContext;
            //__commandContext = services.GetRequiredService<ICommandContext>();
            _staticService = services;
        }

        public  async Task<UserModel> GetUser(ulong userId)
        {
            UserModel user = new UserModel();
            var userModelDTO = new UserModelDTO(_context);
            user = await userModelDTO.GetUser(userId);
            
            return user;
        }

        public  async Task<UserExperience> GetUserExperienceInServer(ulong userId, ulong serverId)
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            UserExperience user = new UserExperience();
            user = await userExperienceDTO.GetUserExperience(userId, serverId);
            
            return user;
        }

        public  async Task<List<UserExperience>> GetAllUserExperiences()
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var users = new List<UserExperience>();
            users = await userExperienceDTO.GetAllUserExperiences();
            
            return users;
        }

        public async Task<Dictionary<ulong, List<ServerModel>>> GetServersFromUser(List<UserExperience> users)
        {
            var serverDTO = new ServerModelDTO(_context);
            var servers = serverDTO.GetAllServers();
            var userDict = new Dictionary<ulong, List<ServerModel>>();
            foreach (var user in users)
            {
                userDict.Add(user.userId, servers.Where(x => x.serverId == user.serverId).ToList());
            }
            return userDict;
        }

        public async Task<UserExperience> GetUserNameExperienceInServer(string username, ulong serverId)
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var users = await userExperienceDTO.GetAllUserExperiencesInServer(serverId);
            var user = users.Where(usr => usr.userName == username).FirstOrDefault();
            return user;
        }

        public async Task<string> CleanUserAtString(string userId)
        {
            return Regex.Replace(userId, "[^.0-9]", "");
        }

        public  async Task<List<UserExperience>> GetAllUserInServer(ulong serverId)
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            List<UserExperience> user = new List<UserExperience>();
            user = await userExperienceDTO.GetAllUserExperiencesInServer(serverId);
            
            return user;
        }

        public async Task<(int, int)> GetUserMoney(IUser user, IGuild guild)
        {
            var GetUser = await GetUserExperienceInServer(user.Id, guild.Id);
            int bank = 0;
            int wallet = 0;
            if (GetUser == null)
            {
                await AddNewUser(user, guild);
                bank = wallet = 500;
            }
            else
            {
                bank = GetUser.bank;
                wallet = GetUser.wallet;
            }
            return (bank, wallet);
        }

        public  async Task AddNewUser(IUser user, IGuild guild)
        {
            var userExists = await GetUser(user.Id);
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var userModelDTO = new UserModelDTO(_context);
            if (userExists == null)
            {
                await userModelDTO.AddUser(user);
                
            }

            var userExpExists = await GetUserExperienceInServer(user.Id, guild.Id);
            if (userExpExists == null)
            {
                await userExperienceDTO.AddUserExperience(user, guild);
            }
        }

        public async Task<bool> CheckValidAmount(IUser user, IGuild guild, int amount)
        {
            bool isValid = false;
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var userExpExists = await GetUserExperienceInServer(user.Id, guild.Id);
            var userInMainTable = await GetUser(user.Id);

            if (userInMainTable == null || userExpExists == null)
            {
                await AddNewUser(user, guild);
                userExpExists = await GetUserExperienceInServer(user.Id, guild.Id);
            }

            if (userExpExists != null)
            {
                if (userExpExists.bank + userExpExists.wallet > amount)
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        public async Task<bool> UpdateMoney(ulong id, IGuild guild, int amount)
        {
            var success = false;
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var userExpExists = await GetUserExperienceInServer(id, guild.Id);

            if (userExpExists != null)
            {
                var totalCash = userExpExists.bank + userExpExists.wallet;
                if (userExpExists.bank + amount > 0)
                {
                    userExpExists.bank += amount;
                    await userExperienceDTO.UpdateUserExperience(userExpExists);
                    success = true;
                }
                else if (userExpExists.wallet + amount > 0)
                {
                    userExpExists.wallet += amount;
                    await userExperienceDTO.UpdateUserExperience(userExpExists);
                    success = true;
                }
                else if (totalCash + amount > 0)
                {
                    totalCash += amount;
                    userExpExists.bank = (int)totalCash / 2;
                    userExpExists.wallet= (int)totalCash / 2;
                    await userExperienceDTO.UpdateUserExperience(userExpExists);
                }
            }
            return success;
        }

        public async Task<bool> UpdateUser(IGuild guild, UserExperience user = null, IUser iusr = null)
        {
            var currUser = new UserExperience();
            if (user == null)
            {
                currUser = await GetUserNameExperienceInServer(iusr.Username, guild.Id);
            }
            else
            {
                currUser = user;
            }

            currUser.messages += 1;
            
            bool leveledUp = false;
            if (currUser.experience >= 100.0)
            {
                currUser.userLevel += 1;
                currUser.experience = 0;
                leveledUp = true;
            }
            if (currUser.messages % 10 == 0)
            {
                currUser.userLevel += 1;
                leveledUp = true;
            }
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            await userExperienceDTO.UpdateUserExperience(currUser);
            return leveledUp;
        }

        public async Task<UserDash> GetUserDashAsync(IUser user, IGuild guild)
        {
            var dash = new UserDash();
            
            using (var dto = new UserDashDTO(_context, _service))
            {
                dash = await dto.GetUserDash(user, guild);
                var dbUser = await GetUserExperienceInServer(user.Id, guild.Id);
                var api = new Apis(_context, _service);
                var colorEnum = new DashColor();
                foreach (var item in dash.items)
                {
                    switch (item.command)
                    {
                        case DashCommand.api:
                            break;
                        case DashCommand.crypto:
                            item.result = await api.CryptoApi(item.value.ToString());
                            break;
                        case DashCommand.image:
                            item.result = item.command.ToString();
                            break;
                        case DashCommand.color:
                            dash.color = (DashColor)Enum.Parse(typeof(DashColor), item.value.ToString());
                            break;
                        case DashCommand.stat:
                            item.result = (string)Enum.Parse(typeof(DashItem), item.value.ToString());
                            break;
                    }
                }
            }

            return dash;
        }

        public async Task<DateTime> ParseTime(string time)
        {
            var exTime = DateTime.Parse(time);
            return exTime;
        }

        public async Task<TimeIncrement> ParseTimeIncrement(string increment)
        {
            var result = (TimeIncrement)Enum.Parse(typeof(TimeIncrement), increment);
            return result;
        }

        public async Task<bool> CheckReminderRepeat(TimeIncrement increment)
        {
            if (increment.ToString().EndsWith("ly"))
            {
                return true;
            }
            return false;
        }

        public async Task AddReminder(ReminderModel model)
        {
            _reminderModelDTO = new ReminderModelDTO(_context, _service);
            await _reminderModelDTO.AddReminder(model);
        }

        public async Task<List<ReminderModel>> GetUserReminders(ulong userId, ulong serverId)
        {
            _reminderModelDTO = new ReminderModelDTO(_context, _service);
            var reminders = await _reminderModelDTO.GetAllReminderModels(sid: serverId, uid: userId);
            return reminders;
        }

        public async Task<List<ReminderModel>> GetServerReminders(ulong serverId)
        {
            _reminderModelDTO = new ReminderModelDTO(_context, _service);
            var reminders = await _reminderModelDTO.GetAllReminderModels(sid: serverId);
            return reminders;
        }

        public async Task<List<ReminderModel>> GetAllReminders()
        {
            _reminderModelDTO = new ReminderModelDTO(_context, _service);
            var reminders = await _reminderModelDTO.GetAllReminderModels();
            return reminders;
        }

        public async Task SendReminder(ReminderModel model)
        {
            var commandContext = _service.GetRequiredService<CommandContext>();
            var user = await commandContext.Channel.GetUserAsync(model.createdById);

            //var user = CommandContext

            StringBuilder sb = new StringBuilder();

            var id = model.createdInServerId;
            sb.Append("\n");
            sb.AppendLine($"{model.value}");
            sb.AppendLine($"{model.additionalInfo}");

            SocketDMChannel DMChannel = (SocketDMChannel)user.GetOrCreateDMChannelAsync().Result;

            await DMChannel.SendMessageAsync(sb.ToString());

            //await user.SendMessageAsync(sb.ToString());
            //await Discord.UserExtensions.SendMessageAsync(user, sb.ToString());
        }

    }
}
