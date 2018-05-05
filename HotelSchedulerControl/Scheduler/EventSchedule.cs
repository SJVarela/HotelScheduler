using System;
using System.Collections.Generic;

namespace HotelSchedulerControl.Scheduler
{
    public class EventSchedule
    {
        public List<ScheduleEvent> Tasks { get; set; } = new List<ScheduleEvent>();
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; } = DateTime.Now.AddDays(20);
        public TimeSpan Now { get { return DateTime.Now - Start; } }

        public void Add(ScheduleEvent task)
        {
            Tasks.Add(task);
        }

        public void Delete(ScheduleEvent task)
        {
            Tasks.Remove(task);
        }

        public int IndexOf(ScheduleEvent task)
        {
            throw new NotImplementedException();
        }

        public void Move(ScheduleEvent task, int offset)
        {
            throw new NotImplementedException();
        }

        public void SetEnd(ScheduleEvent task, DateTime end)
        {
            task.End = end;
        }

        public void SetStart(ScheduleEvent task, DateTime start)
        {
            task.Start = start;
        }
    }
}
