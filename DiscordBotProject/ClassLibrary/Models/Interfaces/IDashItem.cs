namespace ClassLibrary.Models.ContextModels
{
    public interface IDashItem
    {
        int id { get; set; }
        ulong userId { get; set; }
        ulong serverId { get; set; }
        DashCommand command { get; set; }
        string value { get; set; }
        string result { get; set; }
    }
}