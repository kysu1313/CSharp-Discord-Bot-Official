using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class UserDash
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public ulong userId { get; set; }
        public ulong serverId { get; set; }
        public string userName { get; set; }
        public DashColor color { get; set; }

        public virtual IEnumerable<DashItem> items { get; set; }

    }

    public enum DashColor
    {
        Default = 1,
        DarkerGrey = 2,
        DarkGrey = 3,
        LighterGrey = 4,
        DarkRed = 5,
        Red = 6,
        DarkOrange = 7,
        Orange = 8,
        LightOrange = 9,
        Gold = 10,
        LightGrey = 11,
        Magenta = 12,
        DarkPurple = 13,
        Purple = 14,
        DarkBlue = 15,
        Blue = 16,
        DarkGreen = 17,
        Green = 18,
        DarkTeal = 19,
        Teal = 20,
        DarkMagenta = 21
    }
}
