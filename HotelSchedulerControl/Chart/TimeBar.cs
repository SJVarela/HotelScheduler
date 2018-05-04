using System;
using System.Drawing;

namespace HotelSchedulerControl.Chart
{
    public class TimeBar
    {
        public TimeBar()
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
        public TaskFormat Format { get; set; } = new TaskFormat()
        {
            Color = Brushes.Black,
            Border = Pens.Maroon,
            BackFill = Brushes.MediumSlateBlue,
            ForeFill = Brushes.YellowGreen,
            SlackFill = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal, Color.Blue, Color.Transparent)
        };

        /// <summary>
        /// Convert this Task to a descriptive string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Name = {0}, Start = {1}, End = {2}, Duration = {3}]", Name, Start, End, Duration);
        }
    }
}
