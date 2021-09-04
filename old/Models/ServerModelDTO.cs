using DiscBotConsole.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class ServerModelDTO
    {

        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        public ServerModelDTO(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        internal ServerModel AddServer(ServerModel s)
        {
            _context.ServerModels.Add(s);
            _context.SaveChanges();
            
            return s;
        }

        internal List<ServerModel> GetAllUsers()
        {
            List<ServerModel> list = new List<ServerModel>();
            list = _context.ServerModels
                .ToList<ServerModel>();
            
            return list;
        }

    }

}
