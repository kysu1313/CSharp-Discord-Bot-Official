using ClassLibrary.Data;
using BusinessLogic.Helpers;
using Discord;
using System;
using System.Collections.Generic;
// using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Helpers;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models;
using ClassLibrary.Models.ContextModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.ModelDTOs
{
    public class UserDashDTO : IDisposable, IUserDashDTO
    {

        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private bool _disposed = false;

        public UserDashDTO(ApplicationDbContext dbContext, IServiceProvider services)
        {
            _context = dbContext;
            _helper = new Helper(_context, services);
        }

        public async Task AddNewUserDash(IUser user, IGuild guild)
        {
            UserDash dash = new UserDash()
            {
                userId = user.Id,
                serverId = guild.Id,
                userName = user.Username,
                items = new List<DashItem>()
            };
            await _context.UserDashes.AddAsync(dash);
            await _context.SaveChangesAsync();
        }

        // Allow user to add things like statistics / crypto prices / images to their dashboard.
        public async Task AddToDash(IUser user, IGuild guild, DashCommand command, string value)
        {
            var result = await _context.UserDashes.AsNoTracking().FirstOrDefaultAsync(x =>
                        x.userId == user.Id && x.serverId == guild.Id);

            if (result != null)
            {

                DashItem dashItem = new DashItem()
                {
                    userId = user.Id,
                    serverId = guild.Id,
                    command = command,
                    value = value,
                    result = ""
                };

                await _context.DashItems.AddAsync(dashItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserDash> GetUserDash(IUser user, IGuild guild)
        {
            var result = await _context.UserDashes.AsNoTracking().FirstOrDefaultAsync(x => 
                        x.userId == user.Id && x.serverId == guild.Id);
            return result;
        }

        public void Dispose()
        {
            //((IDisposable)_helper).Dispose();
        }
    }
}
