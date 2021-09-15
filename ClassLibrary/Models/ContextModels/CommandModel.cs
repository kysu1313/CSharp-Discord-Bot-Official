using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models.ContextModels
{
    public class CommandModel
    {
        /// <summary>
        /// CommandModel is the general model of all commands.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int commandId { get; set; }

        public ulong serverId { get; set; }
        public string commandName { get; set; }
        public bool enabled { get; set; }
        public ulong modifiedById { get; set; }
        public DateTime dateAdded { get; set; }
        public DateTime? dateModified { get; set; }
        
    }
}