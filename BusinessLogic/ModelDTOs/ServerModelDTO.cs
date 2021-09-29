using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
// using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;

namespace BusinessLogic.ModelDTOs
{
    public class ServerModelDTO : IAsyncDisposable, IDisposable, IServerModelDTO
    {

        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        public ServerModelDTO(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public ServerModel AddServer(ServerModel s)
        {
            var svrs = _context.ServerModels.AsNoTracking().ToList();
            if (svrs.All(x => x.serverId != s.serverId))
            {
                _context.ServerModels.Add(s);
                _context.SaveChanges();
            }
            return s;
        }

        public async Task<List<ServerModel>> GetAllServers()
        {
            var list = new List<ServerModel>();
            list = await _context.ServerModels.AsNoTracking().AsQueryable().ToListAsync();
            
            return list;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            Dispose();
            return;
        }

        public void Dispose()
        {
            GC.Collect();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            GC.Collect();
            return new ValueTask();
        }
    }

}
