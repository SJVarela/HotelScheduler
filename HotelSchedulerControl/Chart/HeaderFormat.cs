using System.Drawing;

namespace HotelSchedulerControl.Scheduler
{
    public class HeaderFormat
    {
        /// <summary>
        /// Font color
        /// </summary>
        public Brush Color { get; set; }
        /// <summary>
        /// Border and line colors
        /// </summary>
        public Pen Border { get; set; }
        /// <summary>
        /// Get or set the lighter color in the gradient
        /// </summary>
        public Color GradientLight { get; set; }
        /// <summary>
        /// Get or set the darker color in the gradient
        /// </summary>
        public Color GradientDark { get; set; }
    }


}