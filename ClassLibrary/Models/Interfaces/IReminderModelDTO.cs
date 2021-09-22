using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.ModelDTOs
{
    public interface IReminderModelDTO
    {
        Task AddReminder(ReminderModel model);
        Task<List<ReminderModel>> GetAllReminders();
        Task<List<ReminderModel>> GetAllReminderModels(Guid? guid = null, ulong? uid = 0, ulong? sid = 0);
        Task<ReminderModel> GetClosestReminderModel(Guid? guid = null, ulong? uid = 0, ulong? sid = 0);
    }
}