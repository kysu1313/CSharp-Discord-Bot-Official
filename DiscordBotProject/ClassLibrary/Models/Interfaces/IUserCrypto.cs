using System;

namespace ClassLibrary.Models.ContextModels
{
    public interface IUserCrypto
    {
        int id { get; set; }
        Guid userId { get; set; }
        string userName { get; set; }
        string privateToken { get; set; }
        string publicToken { get; set; }
    }
}