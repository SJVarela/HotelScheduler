using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HotelSchedulerControl.Scheduler
{
    public class SchedulerEvent
    {
        public SchedulerEvent()
        {
            Start = DateTime.Now;
            End = Start;
            Slack = TimeSpan.Zero;
            Row = -1;
        }

        /// <summary>
        /// Get or set the Name of this Task
        /// </summary>
        public string Name { get; set; }

        public int Row { get; set; }
        /// <summary>
        /// Get the start time of this Task relative to the project start
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Get the end time of this Task relative to the project start
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Get the duration of this Task in days
        /// </summary>
        public TimeSpan Duration { get { return End - Start; } }

        /// <summary>
        /// Get the amount of slack (free float)
        /// </summary>
        public TimeSpan Slack { get; internal set; }

        /// <summary>
        /// Format of the task
        /// </summary>
        public ScheduleEventFormat Format { get; set; } = new ScheduleEventFormat()
        {
            Color = Brushes.Black,
            Border = Pens.Maroon,
            BackFill = Brushes.MediumSlateBlue,
            ForeFill = Brushes.YellowGreen,
            SlackFill = new HatchBrush(HatchStyle.LightDownwardDiagonal, Color.Blue, Color.Transparent)
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
