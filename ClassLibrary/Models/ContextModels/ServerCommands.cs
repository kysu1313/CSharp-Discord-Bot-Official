using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models.ContextModels
{
    public class ServerCommands
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public ulong serverId { get; set; }
        public virtual IEnumerable<CommandModel> commands { get; set; }
        public ulong modifiedById { get; set; }
        public DateTime dateAdded { get; set; }
        public DateTime? dateModified { get; set; }
    }
}