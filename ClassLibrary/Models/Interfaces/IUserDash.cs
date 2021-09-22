using System.Collections.Generic;

namespace ClassLibrary.Models.ContextModels
{
    public interface IUserDash
    {
        int id { get; set; }
        ulong userId { get; set; }
        ulong serverId { get; set; }
        string userName { get; set; }
        DashColor color { get; set; }
        IEnumerable<DashItem> items { get; set; }
    }
}