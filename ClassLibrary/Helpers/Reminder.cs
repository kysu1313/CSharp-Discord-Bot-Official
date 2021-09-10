using ClassLibrary.Data;
using ClassLibrary.Models;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.Helpers
{

    class TimerState
    {
        public int Counter;
    }

    public class Reminder
    {

        private readonly ApplicationDbContext _context; 
        private readonly Helper _helper;
        private Timer timer;
        private static List<ReminderModel> _reminders;
        private static Helper _staticHelper;

        public static Timer _timer { get; set; }

        public Reminder(ApplicationDbContext dbContext, IServiceProvider services)
        {
            _context = dbContext;
            _helper = new Helper(_context, services);
            _staticHelper = new Helper(_context, services);
            if (_timer == null)
                StartTimer();
        }

        public static async Task UpdateReminders(ulong serverId)
        {
            _reminders = await _staticHelper.GetServerReminders(serverId);
        }

        public void StartTimer()
        {
            var timerState = new TimerState { Counter = 0 };

            _timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: timerState,
                dueTime: 1000,
                period: 10000
            );
        }

        private async void TimerTask(object timerState)
        {
            //await TimerTaskAsync(timerState);
        }

        private async Task TimerTaskAsync(object timerState)
        {
            var currentTime = DateTime.Now;
            _reminders = await _staticHelper.GetAllReminders();
            var reminders = _reminders.FindAll(x => x.endDate == false);
            foreach (var item in reminders)
            {
                if (
                    item.executionTime >= currentTime &&
                    item.currentRepeatNumber < item.numberOfRepeats
                    )
                {
                    if (item.currentRepeatNumber > item.numberOfRepeats)
                    {
                        item.endDate = true;
                    }
                    else
                    {
                        // TODO: fix
                        await _helper.SendReminder(item);

                        

                        item.numberOfRepeats++;
                        item.hasExecuted = true;
                    }
                }
            }
        }

    }
}
