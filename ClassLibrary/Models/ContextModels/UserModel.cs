using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.ContextModels
{
    [Serializable]
    public class UserModel : IdentityUser, IUserModel
    {
        public Guid userGuid { get; set; }
        public bool isBotAdmin { get; set; }
        public bool hasLinkedAccount { get; set; }
        public ulong userId { get; set; }
        public string userNameEntry { get; set; }
        public bool slowModeEnabled { get; set; }
        public int slowModeTime { get; set; }
    }
}
