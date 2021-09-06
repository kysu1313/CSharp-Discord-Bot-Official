using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotApi.ModelDTOs
{
    public class ServerModelDTO
    {

        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        public ServerModelDTO(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public ServerModel AddServer(ServerModel s)
        {
            _context.ServerModels.Add(s);
            _context.SaveChanges();
            
            return s;
        }

        public async Task<List<ServerModel>> GetAllServers()
        {
            var list = new List<ServerModel>();
            list = await _context.ServerModels.AsQueryable().ToListAsync();
            
            return list;
        }

    }

}
