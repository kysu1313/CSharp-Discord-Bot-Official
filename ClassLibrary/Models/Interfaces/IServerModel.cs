namespace ClassLibrary.Models.ContextModels
{
    public interface IServerModel
    {
        int id { get; set; }
        ulong serverId { get; set; }
        string serverName { get; set; }
        UserModel? botAdmin { get; set; }
    }
}