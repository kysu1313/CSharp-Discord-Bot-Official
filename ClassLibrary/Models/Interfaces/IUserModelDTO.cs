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
        Task RegisterUser(string name, ulong userId);
        Task LinkUserToServers(ulong userId, Dictionary<ulong, string> serverIds);
        void Dispose();
        ValueTask DisposeAsync();
    }
}