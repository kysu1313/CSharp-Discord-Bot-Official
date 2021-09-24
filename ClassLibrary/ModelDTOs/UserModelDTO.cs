using ClassLibrary.Data;
using ClassLibrary.Models;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.ModelDTOs
{
    public class UserModelDTO : IDisposable, IAsyncDisposable, IUserModelDTO
    {

        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        public UserModelDTO(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddUser(IUser user)
        {
            UserModel newUser = new UserModel()
            {
                userId = user.Id,
                userNameEntry = user.Username,
                slowModeEnabled = false,
                slowModeTime = 0
            };

            await _context.UserModels.AddAsync(newUser);
            await _context.SaveChangesAsync();
            
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            List<UserModel> list;
            list = await _context.UserModels
                .ToListAsync();
            return list;
        }

        public async Task<UserModel> GetUser(ulong userId)
        {
            var user = await _context.UserModels
                .FirstOrDefaultAsync(x => x.userId == userId);
            return user;
        }

        public async Task<UserModel> GetUser(string userName)
        {
            var user = await _context.UserModels
                .FirstOrDefaultAsync(x => x.UserName == userName);
            return user;
        }

        public async Task<UserExperience> GetUserExperience(ulong userId, ulong serverId)
        {
            UserExperience userExp;
            userExp = await _context.UserExperiences
                .FirstOrDefaultAsync(x => 
                    x.userId == userId && 
                    x.serverId == serverId);
            
            return userExp;
        }

        public async Task RegisterUser(ulong userId, ulong serverId)
        {
            var userModel = await GetUser(userId);
            var svr = _context.ServerModels.FirstOrDefault(x => x.serverId == serverId);
            if (svr != null)
            {
                svr.botAdmin = userModel;
                // svr.botAdminId = userId;
            }
            userModel.userId = userId;
            userModel.hasLinkedAccount = true;
            userModel.isBotAdmin = true;
            await _context.SaveChangesAsync();
        }

        public Task LinkUserToServers(ulong userId, Dictionary<ulong, string> serverIds)
        {
            throw new NotImplementedException();
        }

        public async Task LinkUserToServers(ulong userId, ServerModel server)
        {
            var user = _context.UserModels.FirstOrDefault(x => x.userId == userId);
            if (user != null)
            {
                server.botAdmin = user;
                user.isBotAdmin = true;    
            }
            // server.botAdminId = userId;
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
        public virtual ValueTask DisposeAsync()
        {
            Dispose();
            GC.SuppressFinalize(this);
            return new ValueTask();
        }
    }
}
