#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.ContextModels
{
    public class ServerModel : IServerModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public ulong serverId { get; set; }
        public string serverName { get; set; }
        public ulong userIdent { get; set; }
        // public virtual UserModel? botAdmin { get; set; }
    }
}
