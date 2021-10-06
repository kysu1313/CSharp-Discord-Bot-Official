using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;
using Discord.WebSocket;

namespace ClassLibrary.ModelDTOs
{
    public interface ICommandModelDTO
    {
        Task AddCommands(List<string> commands, SocketUser user, SocketGuild guild);
        Task<IEnumerable<CommandModel>> GetCommands(ulong serverId);
        Task<bool> GetCommandStatus(string name, ulong serverId);
        Task UpdateCommandUses(string name, int newUses, ulong serverId);
        Task UpdateCommandStatus(string name, bool enabled, ulong serverId, ulong userId);
        ValueTask DisposeAsync();
        void Dispose();
    }
}