﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class UserExperience
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public ulong serverId { get; set; }
        public ulong userId { get; set; }
        public string userName { get; set; }
        public int bank { get; set; }
        public int wallet { get; set; }
        public int messages { get; set; }
        public int userLevel { get; set; }
        public int luck { get; set; }
        public float experience { get; set; }
        public int emojiSent { get; set; }
        public int reactionsReceived { get; set; }
        public DateTime dateUpdated { get; set; }
    }
}