namespace ClassLibrary.Models.ContextModels
{
    public interface IStatModel
    {
        int id { get; set; }
        ulong userId { get; set; }
        ulong serverId { get; set; }
        int bank { get; set; }
        int wallet { get; set; }
        int messages { get; set; }
        int userLevel { get; set; }
        int luck { get; set; }
        float experience { get; set; }
        int emojiSent { get; set; }
        int reactionsReceived { get; set; }
    }
}