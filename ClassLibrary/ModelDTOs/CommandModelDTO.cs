using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.Helpers;
using ClassLibrary.Models.ContextModels;
using Discord;
using Discord.WebSocket;

namespace ClassLibrary.ModelDTOs
{
    public class CommandModelDTO : IAsyncDisposable, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private bool _disposed = false;

        public CommandModelDTO(ApplicationDbContext dbContext, IServiceProvider services)
        {
            _context = dbContext;
            _helper = new Helper(_context, services);
        }

        public async Task AddCommands(List<string> commands, SocketUser user, SocketGuild guild)
        {
            List<CommandModel> cmdLst = new List<CommandModel>();
            foreach (var command in commands)
            {
                CommandModel cmd = new CommandModel()
                {
                    serverId = guild.Id,
                    commandName = command,
                    enabled = true,
                    modifiedById = user.Id,
                    dateAdded = DateTime.Now,
                    dateModified = null
                };
                cmdLst.Add(cmd);
            }

            var serverCmds = await _context.ServerCommandModels
                .FirstOrDefaultAsync(x => x.serverId == guild.Id);

            if (serverCmds == null)
            {
                ServerCommands svcmd = new ServerCommands()
                {
                    serverId = guild.Id,
                    commands = cmdLst,
                    modifiedById = user.Id,
                    dateAdded = DateTime.Now,
                    dateModified = null
                };
                await _context.ServerCommandModels.AddAsync(svcmd);
            }
            else
            {
                serverCmds.commands = cmdLst;
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommandModel>> GetCommands(ulong serverId)
        {
            var svr = (await _context.ServerCommandModels.ToListAsync())
                .FirstOrDefault(x => x.serverId == serverId);
            var cmds = svr == null || svr.commands == null ? 
                new List<CommandModel>() : svr.commands;

            return cmds;
        }

        public async Task<bool> GetCommandStatus(string name, ulong serverId)
        {
            var svr = (await _context.ServerCommandModels.ToListAsync())
                .FirstOrDefault(x => 
                    x.serverId == serverId);
            if (svr != null)
            {
                var exists = svr.commands.FirstOrDefault(x => 
                    x.commandName == name);
                return exists == null ? false : exists.enabled;
            }

            return false;
        }

        public async Task UpdateCommandStatus(string name, bool enabled, ulong serverId)
        {
            var cmdSvr = await _context.ServerCommandModels
                .FirstOrDefaultAsync(x => 
                    x.serverId == serverId);
            var cmd = cmdSvr.commands.FirstOrDefault(x => 
                x.commandName.ToLower() == name.ToLower());
            if (cmd != null)
            {
                cmd.enabled = enabled;
            }

            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            _helper?.Dispose();
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _helper?.Dispose();
            }
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            _helper.Dispose();
            return;
        }
    }
}