using DiscBotConsole.Data;
using DiscBotConsole.Helpers;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class UserExperienceDTO : IDisposable
    {

        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private bool _disposed = false;

        internal UserExperienceDTO(ApplicationDbContext dbContext, IServiceProvider services)
        {
            _context = dbContext;
            _helper = new Helper(_context, services);
        }

        internal async Task<UserExperience> AddUserExperience(IUser user, IGuild guild)
        {
            UserExperience newUserExp = new UserExperience()
            {
                serverId = guild.Id,
                userId = user.Id,
                userName = user.Username,
                bank = _helper.STARTING_MONEY,
                wallet = _helper.STARTING_MONEY,
                messages = 0,
                userLevel = 0,
                experience = 1,
                emojiSent = 0,
                reactionsReceived = 0,
                dateUpdated = DateTime.UtcNow
            };

            await _context.UserExperiences.AddAsync(newUserExp);
            await _context.SaveChangesAsync();
            
            return newUserExp;
        }

        internal async Task UpdateUserExperience(UserExperience user)
        {
            _context.UserExperiences.Update(user);
            await _context.SaveChangesAsync();

        }

        internal async Task<List<UserExperience>> GetAllUserExperiencesInServer(ulong serverId)
        {
            var list = new List<UserExperience>();
            var result = new List<UserExperience>();
            var tmp = await _context.UserExperiences.ToListAsync();
            list = await _context.UserExperiences.ToListAsync();
            if (tmp.Any())
            {
                result = tmp.AsQueryable().Where(x => x.serverId == serverId).ToList();
            }
            
            return (List<UserExperience>)tmp;
        }

        internal async Task<UserExperience> GetUserExperience(ulong userId, ulong serverId)
        {
            UserExperience user = new UserExperience();
            user = await _context.UserExperiences
                            .FirstOrDefaultAsync(x => 
                                x.userId == userId && 
                                x.serverId == serverId)
                            .ConfigureAwait(false);
            
            return user;
        }

        public void Dispose()
        {
            ((IDisposable)_helper).Dispose();
        }
    }
}
