using System;

namespace ClassLibrary.Models.ContextModels
{
    [Serializable]
    public class UserStatsModel : IUserStatsModel
    {
        public int id { get; set; }
        public ulong userId { get; set; }
        public ulong serverId { get; set; }
        public virtual StatModel stats { get; set; }
    }
}
