using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.ContextModels
{
    public class StatModel : IStatModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public ulong userId { get; set; }
        public ulong serverId { get; set; }

        public int bank { get; set; }
        public int wallet { get; set; }
        public int messages { get; set; }
        public int userLevel { get; set; }
        public int luck { get; set; }
        public float experience { get; set; }
        public int emojiSent { get; set; }
        public int reactionsReceived { get; set; }
    }
}
