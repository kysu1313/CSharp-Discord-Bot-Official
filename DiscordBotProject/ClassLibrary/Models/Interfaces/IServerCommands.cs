using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.ContextModels
{
    public interface IServerCommands
    {
        ulong serverCommandId { get; set; }
        IEnumerable<CommandModel> commands { get; set; }
        ulong modifiedById { get; set; }
        DateTime dateAdded { get; set; }
        DateTime? dateModified { get; set; }
    }
}