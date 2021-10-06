using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;
using Discord;

namespace ClassLibrary.Models
{
    public interface IUserExperienceDTO
    {
        Task<UserExperience> AddUserExperience(IUser user, IGuild guild);
        Task UpdateUserExperience(UserExperience user);
        Task<List<UserExperience>> GetAllUserExperiences();
        Task<List<UserExperience>> GetAllUserExperiencesInServer(ulong serverId);
        Task<UserExperience> GetUserExperience(ulong userId, ulong serverId);
        void Dispose();
        ValueTask DisposeAsync();
    }
}