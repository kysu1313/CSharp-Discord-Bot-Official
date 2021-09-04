﻿using DiscBotConsole.Data;
using DiscBotConsole.Helpers;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class UserDashDTO : IDisposable
    {

        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private bool _disposed = false;

        internal UserDashDTO(ApplicationDbContext dbContext, IServiceProvider services)
        {
            _context = dbContext;
            _helper = new Helper(_context, services);
        }

        internal async Task AddNewUserDash(IUser user, IGuild guild)
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
        internal async Task AddToDash(IUser user, IGuild guild, DashCommand command, string value)
        {
            var result = await _context.UserDashes.FirstOrDefaultAsync(x =>
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

        internal async Task<UserDash> GetUserDash(IUser user, IGuild guild)
        {
            var result = await _context.UserDashes.FirstOrDefaultAsync(x => 
                        x.userId == user.Id && x.serverId == guild.Id);
            return result;
        }

        public void Dispose()
        {
            ((IDisposable)_helper).Dispose();
        }
    }
}
