using System;

namespace HotelSchedulerControl.Chart
{
    public class Task
    {
        public Task()
        {
            Start = TimeSpan.Zero;
            End = new TimeSpan(1, 0, 0, 0);
            Duration = new TimeSpan(1, 0, 0, 0);
            Slack = TimeSpan.Zero;
        }

        /// <summary>
        /// Get or set the Name of this Task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get the start time of this Task relative to the project start
        /// </summary>
        public TimeSpan Start { get; internal set; }

        /// <summary>
        /// Get the end time of this Task relative to the project start
        /// </summary>
        public TimeSpan End { get; internal set; }

        /// <summary>
        /// Get the duration of this Task in days
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Get the amount of slack (free float)
        /// </summary>
        public TimeSpan Slack { get; internal set; }

        /// <summary>
        /// Format of the task
        /// </summary>
        public TaskFormat Format { get; set; }
        /// <summary>
        /// Convert this Task to a descriptive string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Name = {0}, Start = {1}, End = {2}, Duration = {3}, Complete = {4}]", Name, Start, End, Duration);
        }
    }
}
