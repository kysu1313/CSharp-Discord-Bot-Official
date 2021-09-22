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
    public class CommandModelDTO : IAsyncDisposable, IDisposable, ICommandModelDTO
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
                .FirstOrDefaultAsync(x => x.serverCommandId == guild.Id);

            if (serverCmds == null)
            {
                ServerCommands svcmd = new ServerCommands()
                {
                    serverCommandId = guild.Id,
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
                .FirstOrDefault(x => x.serverCommandId == serverId);
            var cmds = svr == null ? new List<CommandModel>() : 
                (await _context.CommandModels.ToListAsync())
                .FindAll(x => x.serverId == svr.serverCommandId);
            return cmds;
        }

        public async Task<bool> GetCommandStatus(string name, ulong serverId)
        {
            var svr = (await _context.ServerCommandModels.ToListAsync())
                .FirstOrDefault(x => 
                    x.serverCommandId == serverId);
            if (svr != null)
            {
                var exists = svr.commands.FirstOrDefault(x => 
                    x.commandName == name);
                return exists == null ? false : exists.enabled;
            }

            return false;
        }

        public async Task UpdateCommandUses(string name, int newUses, ulong serverId)
        {
            var cmds = await GetCommands(serverId);
            var cmd = cmds.FirstOrDefault(x => x.commandName == name);
            if (cmd == null)
            {
                return;
            }
            cmd.totalUses += newUses;
        }

        public async Task UpdateCommandStatus(string name, bool enabled, ulong serverId, ulong userId)
        {
            var cmdSvr = await _context.ServerCommandModels
                .FirstOrDefaultAsync(x => 
                    x.serverCommandId == serverId);
            var cmd = cmdSvr.commands.FirstOrDefault(x => 
                x.commandName.ToLower() == name.ToLower());
            if (cmd != null)
            {
                cmd.enabled = enabled;
                cmd.dateModified = DateTime.Now;
                cmd.modifiedById = userId;
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