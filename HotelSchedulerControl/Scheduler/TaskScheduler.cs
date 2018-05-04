using HotelSchedulerControl.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelSchedulerControl.Scheduler
{
    public class TaskScheduler
    {
        public List<SchedulerEvent> Tasks { get; set; } = new List<SchedulerEvent>();
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; } = DateTime.Now.AddDays(20);
        public TimeSpan Now { get { return DateTime.Now - Start; } }

        public void Add(SchedulerEvent task)
        {
            Tasks.Add(task);
        }

        public void Delete(SchedulerEvent task)
        {
            Tasks.Remove(task);
        }

        public int IndexOf(SchedulerEvent task)
        {
            throw new NotImplementedException();
        }

        public void Move(SchedulerEvent task, int offset)
        {
            throw new NotImplementedException();
        }

        public void SetEnd(SchedulerEvent task, DateTime end)
        {
            task.End = end;
        }

        public void SetStart(SchedulerEvent task, DateTime start)
        {
            task.Start = start;
        }
    }
}
