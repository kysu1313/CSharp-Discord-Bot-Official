using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.ContextModels
{
    [Serializable]
    public class UserCrypto : IUserCrypto
    {
        public int id { get; set; }
        public Guid userId { get; set; }
        public string userName { get; set; }
        public string privateToken { get; set; }
        public string publicToken { get; set; }
    }
}
