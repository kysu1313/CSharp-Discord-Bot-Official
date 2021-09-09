using ClassLibrary.Helpers;
using ClassLibrary.Data;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary.ModelDTOs
{
    public class ReminderModelDTO
    {

        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private bool _disposed = false;

        public ReminderModelDTO(ApplicationDbContext dbContext, IServiceProvider services)
        {
            _context = dbContext;
            _helper = new Helper(_context, services);
        }

        public async Task AddReminder(ReminderModel model)
        {
            await _context.ReminderModels.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReminderModel>> GetAllReminders()
        {
            var reminders = await _context.ReminderModels.ToListAsync();
            return reminders;
        }

        public async Task<List<ReminderModel>> GetAllReminderModels(Guid? guid = null, ulong? uid = 0, ulong? sid = 0)
        {
            var reminders = await _context.ReminderModels.ToListAsync();
            reminders.OrderByDescending(s => s.executionTime);

            if (guid != null)
            {
                reminders = reminders.FindAll(x => x.reminderId == guid);
            }
            else if (uid != 0)
            {
                reminders = reminders.FindAll(x => x.createdById == uid);
            }
            else if (sid != 0)
            {
                reminders = reminders.FindAll(x => x.createdInServerId == sid);
            }

            return reminders;
        }

        public async Task<ReminderModel> GetClosestReminderModel(Guid? guid = null, ulong? uid = 0, ulong? sid = 0)
        {
            var reminders = await _context.ReminderModels.ToListAsync();
            reminders.OrderByDescending(s => s.executionTime);
            var reminder = new ReminderModel();

            if (guid != null)
            {
                reminder = reminders.FirstOrDefault(x => x.reminderId == guid);
            }
            else if (uid != 0)
            {
                reminder = reminders.FirstOrDefault(x => x.createdById == uid);
            }
            else if (sid != 0)
            {
                reminder = reminders.FirstOrDefault(x => x.createdInServerId == sid);
            }
            return reminder;
        }

    }
}
