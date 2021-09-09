using ClassLibrary.Data;
using ClassLibrary.Models;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DataContext
{
    public class UserModelDTO : IDisposable
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
                userName = user.Username,
                slowModeEnabled = false,
                slowModeTime = 0
            };

            await _context.UserModels.AddAsync(newUser);
            await _context.SaveChangesAsync();
            
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            List<UserModel> list = new List<UserModel>();
            list = await _context.UserModels
                .ToListAsync();
            
            return list;
        }

        public async Task<UserModel> GetUser(ulong userId)
        {
            UserModel user = new UserModel();
            user = await _context.UserModels
                .FirstOrDefaultAsync(x => x.userId == userId);
            
            return user;
        }

        public async Task<UserExperience> GetUserExperience(ulong userId, ulong serverId)
        {
            UserExperience userExp = new UserExperience();
            userExp = await _context.UserExperiences
                .FirstOrDefaultAsync(x => 
                    x.userId == userId && 
                    x.serverId == serverId);
            
            return userExp;
        }

        public void Dispose()
        {
        }

        //~UserModelDTO() => Dispose(false);

        //// Public implementation of Dispose pattern callable by consumers.
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //// Protected implementation of Dispose pattern.
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (_disposed)
        //    {
        //        return;
        //    }

        //    if (disposing)
        //    {
        //        // TODO: dispose managed state (managed objects).
        //        //Dispose();
        //    }

        //    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //    // TODO: set large fields to null.

        //    _disposed = true;
        //}
    }
}
