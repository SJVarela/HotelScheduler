using HotelSchedulerControl.Chart;
using System;
using System.Collections.Generic;

namespace HotelSchedulerControl.Scheduler
{
    /// <summary>
    /// ProjectManager interface
    /// </summary>
    /// <typeparam name="T">Task class type</typeparam>
    /// <typeparam name="R">Resource class type</typeparam>
    public interface IScheduler<T, R>
        where T : TimeBar
        where R : class
    {
        /// <summary>
        /// Add task to project manager
        /// </summary>
        /// <param name="task"></param>
        void Add(T task);
        /// <summary>
        /// Delete task from project manager
        /// </summary>
        /// <param name="task"></param>
        void Delete(T task);
        /// <summary>
        /// Move the specified task by offset positions in the task enumeration
        /// </summary>
        /// <param name="task"></param>
        /// <param name="offset"></param>
        void Move(T task, int offset);
        /// <summary>
        /// Enumerate through all the tasks in the ProjectManager.
        /// If there are no change to groups and no add/delete tasks, the order between consecutive calls is preserved.
        /// </summary>
        IEnumerable<T> Tasks { get; }
        /// <summary>
        /// Set the start time of the specified task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="start">Number of timescale units after ProjectManager.Start</param>
        void SetStart(T task, TimeSpan start);
        /// <summary>
        /// Set the end time of the specified task. Duration is automatically adjusted to satisfy.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="end">Number of timescale units after ProjectManager.Start</param>
        void SetEnd(T task, TimeSpan end);
        /// <summary>
        /// Set the duration of the specified task from start to end.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="duration">Number of timescale units between ProjectManager.Start</param>
        void SetDuration(T task, TimeSpan duration);
        /// <summary>
        /// Set the "now" time. Its value is the number of timescale units after the start time.
        /// </summary>
        TimeSpan Now { get; }
        /// <summary>
        /// Set the start date of the project.
        /// </summary>
        DateTime Start { get; set; }
        /// <summary>
        /// Get the zero-based index of the specified task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        int IndexOf(T task);
    }
}
