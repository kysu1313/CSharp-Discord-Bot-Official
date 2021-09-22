using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;
using Discord;

namespace ClassLibrary.ModelDTOs
{
    public interface IStatModelDTO
    {
        Task AddStats(IUser user, IGuild server);
        Task<StatModel> GetUserStat(IUser user, IGuild server);

        Task UpdateStat(IUser user, IGuild server, int? bank = null, 
            int? wallet = null, int? messages = null, int? userLevel = null, 
            int? luck = null, float? experience = null, int? emojiSent = null,
            int? reactionsReceived = null);
    }
}