﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.ContextModels
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public Guid userGuid { get; set; }
        public ulong userId { get; set; }
        public string userName { get; set; }
        public bool slowModeEnabled { get; set; }
        public int slowModeTime { get; set; }
    }
}
