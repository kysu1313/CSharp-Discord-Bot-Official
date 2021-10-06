using ClassLibrary.Data;
using ClassLibrary.Models;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.ModelDTOs
{
    public class StatModelDTO : IStatModelDTO
    {

        private readonly ApplicationDbContext _context;

        public StatModelDTO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddStats(IUser user, IGuild server)
        {
            var stat = new StatModel()
            {
                userId = user.Id,
                serverId = server.Id,
                bank = 0,
                wallet = 0, 
                messages = 0, 
                userLevel = 0,
                luck = 0, 
                experience = 0, 
                emojiSent = 0,
                reactionsReceived = 0
            };
            var st = await _context.StatModels.ContainsAsync(stat);
            if (!st)
            {
                await _context.StatModels.AddAsync(stat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<StatModel> GetUserStat(IUser user, IGuild server)
        {
            var stat = await _context.StatModels
                .FirstOrDefaultAsync(x => x.userId == user.Id && x.serverId == server.Id);
            return stat;
        }

        public async Task UpdateStat(IUser user, IGuild server, int? bank = null, 
            int? wallet = null, int? messages = null, int? userLevel = null, 
            int? luck = null, float? experience = null, int? emojiSent = null,
            int? reactionsReceived = null)
        {
            var stat = await _context.StatModels
                .FirstOrDefaultAsync(x => x.userId == user.Id && x.serverId == server.Id);
            stat.bank = bank == null ? stat.bank : (int)bank;
            stat.wallet = wallet == null ? stat.wallet : (int)wallet;
            stat.messages = messages == null ? stat.messages : (int)messages;
            stat.userLevel = userLevel == null ? stat.userLevel : (int)userLevel;
            stat.luck = luck == null ? stat.luck : (int)luck;
            stat.experience = experience == null ? stat.experience : (float)experience;
            stat.emojiSent = emojiSent == null ? stat.emojiSent : (int)emojiSent;
            stat.bank = reactionsReceived == null ? stat.reactionsReceived : (int)reactionsReceived;
            await _context.SaveChangesAsync();
        }


    }
}
