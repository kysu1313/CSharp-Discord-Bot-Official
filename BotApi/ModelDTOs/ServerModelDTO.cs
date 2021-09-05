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
        
        public List<ServerModel> GetAllServers()
        {
            List<ServerModel> list = new List<ServerModel>();
            list = _context.ServerModels
                .ToList<ServerModel>();
            
            return list;
        }

    }

}
