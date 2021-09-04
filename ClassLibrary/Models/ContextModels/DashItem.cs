using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class DashItem
    {
        public int id { get; set; }
        public ulong userId { get; set; }
        public ulong serverId { get; set; }
        public DashCommand command { get; set; }
        public string value { get; set; }
        public string result { get; set; }
    }

    public enum DashCommand
    {
        image,
        crypto,
        stat,
        api,
        color
    }
}
