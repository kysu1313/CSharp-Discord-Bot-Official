using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.ModelDTOs
{
    public interface IServerModelDTO
    {
        ServerModel AddServer(ServerModel s);
        Task<List<ServerModel>> GetAllServers();
        void Dispose();
        ValueTask DisposeAsync();
    }
}