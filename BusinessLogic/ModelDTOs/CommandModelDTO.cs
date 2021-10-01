using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using BusinessLogic.Helpers;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using Discord;
using Discord.WebSocket;

namespace BusinessLogic.ModelDTOs
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

        /// <summary>
        /// Adds a command to a server.
        /// This is mainly used to link commands to a newly added server.
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="user"></param>
        /// <param name="guild"></param>
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

        /// <summary>
        /// Gets all the commands in a server given the server's ID.
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CommandModel>> GetCommands(ulong serverId)
        {
            var svr = (await _context.ServerCommandModels.ToListAsync())
                .FirstOrDefault(x => x.serverCommandId == serverId);
            var cmds = svr == null ? new List<CommandModel>() : 
                (await _context.CommandModels.ToListAsync())
                .FindAll(x => x.serverId == svr.serverCommandId);
            return cmds;
        }

        /// <summary>
        /// Returns the status of a command given it's name and server ID.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the usage count of a command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newUses"></param>
        /// <param name="serverId"></param>
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

        /// <summary>
        /// Enables or disables the given command.
        /// Updates the modified properties of the command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enabled"></param>
        /// <param name="serverId"></param>
        /// <param name="userId"></param>
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
        
        /// <summary>
        /// Returns the server command model given it's server command ID
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<ServerCommands?> GetServerCommands(ulong serverCid)
        {
            var cmdSvr = await _context.ServerCommandModels
                .FirstOrDefaultAsync(x => 
                    x.serverCommandId == serverCid);
            return cmdSvr;
        }
        
        /// <summary>
        /// Returns the command model given it's command ID
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public async Task<CommandModel?> GetCommand(int commandId)
        {
            var cmd = (await _context.CommandModels.ToListAsync())
                .FirstOrDefault(x => x.commandId == commandId);
            return cmd;
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