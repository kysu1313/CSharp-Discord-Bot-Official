using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;
using Discord;

namespace ClassLibrary.ModelDTOs
{
    public interface IUserModelDTO
    {
        Task AddUser(IUser user);
        Task<List<UserModel>> GetAllUsers();
        Task<UserModel> GetUser(ulong userId);
        Task<UserModel> GetUser(string userName);
        Task<UserExperience> GetUserExperience(ulong userId, ulong serverId);
        Task RegisterUser(ulong userId, ulong serverId);
        Task LinkUserToServers(ulong userId, ServerModel server);
        void Dispose();
        ValueTask DisposeAsync();
    }
}