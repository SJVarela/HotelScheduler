using System.Drawing;

namespace HotelSchedulerControl.Scheduler
{
    public class ScheduleEventFormat
    {
        /// <summary>
        /// Get or set Task outline color
        /// </summary>
        public Pen Border { get; set; }

        /// <summary>
        /// Get or set Task background color
        /// </summary>
        public Brush BackFill { get; set; }

        /// <summary>
        /// Get or set Task foreground color
        /// </summary>
        public Brush ForeFill { get; set; }

        /// <summary>
        /// Get or set Task font color
        /// </summary>
        public Brush Color { get; set; }

        /// <summary>
        /// Get or set the brush for slack bars
        /// </summary>
        public Brush SlackFill { get; set; }
    }
}