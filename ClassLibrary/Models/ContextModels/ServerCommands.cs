using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models.ContextModels
{
    /// <summary>
    /// ServerCommands link a group of commands to a server.
    /// This is used for the dashboard.
    /// </summary>
    public class ServerCommands : IServerCommands
    {
        [Key]
        public ulong serverCommandId { get; set; }
        public IEnumerable<CommandModel> commands { get; set; }
        public ulong modifiedById { get; set; }
        public DateTime dateAdded { get; set; }
        public DateTime? dateModified { get; set; }
    }
}

