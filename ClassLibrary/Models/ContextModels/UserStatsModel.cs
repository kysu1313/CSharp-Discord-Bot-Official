using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.Models
{
    public class UserStatsModel
    {
        public int id { get; set; }
        public ulong userId { get; set; }
        public ulong serverId { get; set; }
        public virtual StatModel stats { get; set; }
    }
}
