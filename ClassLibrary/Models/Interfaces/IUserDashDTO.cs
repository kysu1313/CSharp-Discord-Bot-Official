using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;
using Discord;

namespace ClassLibrary.ModelDTOs
{
    public interface IUserDashDTO
    {
        Task AddNewUserDash(IUser user, IGuild guild);
        Task AddToDash(IUser user, IGuild guild, DashCommand command, string value);
        Task<UserDash> GetUserDash(IUser user, IGuild guild);
        void Dispose();
    }
}