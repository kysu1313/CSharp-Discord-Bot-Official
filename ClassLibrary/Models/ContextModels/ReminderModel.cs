using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class ReminderModel
    {

        private DateTime _added;

        [Key]
        [DataMember(Name = "Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [DataMember(Name = "ReminderId")]
        public Guid reminderId { get; set; }
        [DataMember(Name = "UserId")]
        public ulong createdById { get; set; }
        [DataMember(Name = "ServerId")]
        public ulong createdInServerId { get; set; }
        [DataMember(Name = "Value")]
        public string value { get; set; }
        [DataMember(Name = "AdditionalInfo")]
        public string? additionalInfo { get; set; }
        [DataMember(Name = "HasExecuted")]
        public bool hasExecuted { get; set; }
        [DataMember(Name = "ShouldRepeat")]
        public bool? shouldRepeat { get; set; }
        [DataMember(Name = "NumberOfRepeats")]
        public int? numberOfRepeats { get; set; }
        [DataMember(Name = "CurrentRepeatNumber")]
        public int? currentRepeatNumber { get; set; }
        [DataMember(Name = "RepeatIncrement")]
        public TimeIncrement? repeatIncrement { get; set; }
        [DataMember(Name = "EndDate")]
        public bool? endDate { get; set; }

        [DataMember(Name = "TimeAdded")]
        public DateTime timeAdded { 
            get
            {
                return _added;
            } 
            set
            {
                _added = DateTime.Now;
            }
                
        }

        [DataMember(Name = "ExecutionTime")]
        public DateTime executionTime { get; set; }
        [DataMember(Name = "ExecutionIncrement")]
        public TimeIncrement? executionIncrement { get; set; }

    }

    public enum TimeIncrement
    {
        today,
        minute = 0,
        minutes = 1,
        day = 2,
        daily = 3,
        week = 4,
        weekly = 5,
        month = 6,
        monthly = 7,
        year = 8,
        yearly = 9
    }
}
