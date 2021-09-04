using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class CryptoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string coinName { get; set; }
        public float price1 { get; set; }
        public float price2 { get; set; }
        public float price3 { get; set; }
        public float price4 { get; set; }
        public float price5 { get; set; }
        public DateTime dateUpdated { get; set; }
    }
}

