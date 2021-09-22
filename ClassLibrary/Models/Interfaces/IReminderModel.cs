using System;

namespace ClassLibrary.Models.ContextModels
{
    public interface IReminderModel
    {
        int id { get; set; }
        Guid reminderId { get; set; }
        ulong createdById { get; set; }
        ulong createdInServerId { get; set; }
        string value { get; set; }
        string? additionalInfo { get; set; }
        bool hasExecuted { get; set; }
        bool? shouldRepeat { get; set; }
        int? numberOfRepeats { get; set; }
        int? currentRepeatNumber { get; set; }
        TimeIncrement? repeatIncrement { get; set; }
        bool? endDate { get; set; }
        DateTime timeAdded { get; set; }
        DateTime executionTime { get; set; }
        TimeIncrement? executionIncrement { get; set; }
    }
}