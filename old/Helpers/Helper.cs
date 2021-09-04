using DiscBotConsole.Data;
using DiscBotConsole.DataContext;
using DiscBotConsole.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Helpers
{
    public class Helper
    {
        public int STARTING_MONEY = 500;
        public int MAX_USER_LEVEL = 100;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _service;
        private readonly UserExperienceDTO _userExperienceDTO;
        private readonly UserModelDTO _userModelDTO;
        private readonly ServerModelDTO _serverModelDTO;

        public Helper(ApplicationDbContext context, IServiceProvider services)
        {
            _context = context;
            _service = services;
            _config = services.GetRequiredService<IConfiguration>();
        }

        public  async Task<UserModel> getUser(ulong userId)
        {
            UserModel user = new UserModel();
            var userModelDTO = new UserModelDTO(_context);
            user = await userModelDTO.GetUser(userId);
            
            return user;
        }

        public  async Task<UserExperience> getUserExperienceInServer(ulong userId, ulong serverId)
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            UserExperience user = new UserExperience();
            user = await userExperienceDTO.GetUserExperience(userId, serverId);
            
            return user;
        }

        public async Task<UserExperience> getUserNameExperienceInServer(string username, ulong serverId)
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var users = await userExperienceDTO.GetAllUserExperiencesInServer(serverId);
            var user = users.Where(usr => usr.userName == username).FirstOrDefault();
            return user;
        }

        public  async Task<List<UserExperience>> getAllUserInServer(ulong serverId)
        {
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            List<UserExperience> user = new List<UserExperience>();
            user = await userExperienceDTO.GetAllUserExperiencesInServer(serverId);
            
            return user;
        }

        public async Task<(int, int)> GetUserMoney(IUser user, IGuild guild)
        {
            var getUser = await getUserExperienceInServer(user.Id, guild.Id);
            int bank = 0;
            int wallet = 0;
            if (getUser == null)
            {
                await AddNewUser(user, guild);
                bank = wallet = 500;
            }
            else
            {
                bank = getUser.bank;
                wallet = getUser.wallet;
            }
            return (bank, wallet);
        }

        public  async Task AddNewUser(IUser user, IGuild guild)
        {
            var userExists = await getUser(user.Id);
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var userModelDTO = new UserModelDTO(_context);
            if (userExists == null)
            {
                await userModelDTO.AddUser(user);
                
            }

            var userExpExists = await getUserExperienceInServer(user.Id, guild.Id);
            if (userExpExists == null)
            {
                await userExperienceDTO.AddUserExperience(user, guild);
            }
        }

        public async Task<bool> CheckValidAmount(IUser user, IGuild guild, int amount)
        {
            bool isValid = false;
            var userExperienceDTO = new UserExperienceDTO(_context, _service);
            var userExpExists = await getUserExperienceInServer(user.Id, guild.Id);
            var userInMainTable = await getUser(user.Id);

            if (userInMainTable == null || userExpExists == null)
            {
                await AddNewUser(user, guild);
                userExpExists = await getUserExperienceInServer(user.Id, guild.Id);
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
            var userExpExists = await getUserExperienceInServer(id, guild.Id);

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
                currUser = await getUserNameExperienceInServer(iusr.Username, guild.Id);
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
                var dbUser = await getUserExperienceInServer(user.Id, guild.Id);
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

    }
}
