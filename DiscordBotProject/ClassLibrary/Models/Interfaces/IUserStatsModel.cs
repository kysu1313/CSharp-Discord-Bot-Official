using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.Models
{
    public interface IUserStatsModel
    {
        int id { get; set; }
        ulong userId { get; set; }
        ulong serverId { get; set; }
        StatModel stats { get; set; }
    }
}