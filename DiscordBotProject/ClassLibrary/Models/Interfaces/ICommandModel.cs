using System;

namespace ClassLibrary.Models.ContextModels
{
    public interface ICommandModel
    {
        /// <summary>
        /// CommandModel is the general model of all commands.
        /// </summary>
        int commandId { get; set; }

        ulong serverId { get; set; }
        string commandName { get; set; }
        bool enabled { get; set; }
        ulong modifiedById { get; set; }
        DateTime dateAdded { get; set; }
        DateTime? dateModified { get; set; }
        int totalUses { get; set; }
    }
}