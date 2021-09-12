using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ClassLibrary.Models.ContextModels
{
    public class UserModel : IdentityUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userEntryId { get; set; }

        public Guid userGuid { get; set; }
        public ulong userId { get; set; }
        public string userNameEntry { get; set; }
        public bool slowModeEnabled { get; set; }
        public int slowModeTime { get; set; }
    }
}
