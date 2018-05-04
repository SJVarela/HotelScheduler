using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotelSchedulerControl.Scheduler
{
    /// <summary>
    /// Provides data for ChartPaintEvent
    /// </summary>
    public class ChartPaintEventArgs : PaintEventArgs
    {
        /// <summary>
        /// Get the chart that for this event
        /// </summary>
        public SchedulerControl Chart { get; private set; }

        /// <summary>
        /// Initialize a new instance of ChartPaintEventArgs with the PaintEventArgs graphics and clip rectangle, and the chart itself.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="clipRect"></param>
        /// <param name="chart"></param>
        public ChartPaintEventArgs(Graphics graphics, Rectangle clipRect, SchedulerControl chart)
            : base(graphics, clipRect)
        {
            this.Chart = chart;
        }
    }

    /// <summary>
    /// Provides data for ChartPaintEvent
    /// </summary>
    public class HeaderPaintEventArgs : ChartPaintEventArgs
    {
        /// <summary>
        /// Get or set the font to use for drawing the text on the header
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// Get or set the header formatting
        /// </summary>
        public HeaderFormat Format { get; set; }

        /// <summary>
        /// Initialize a new instance of HeaderPaintEventArgs with the editable default font and header format
        /// </summary>
        public HeaderPaintEventArgs(Graphics graphics, Rectangle clipRect, SchedulerControl chart, Font font, HeaderFormat format)
            : base(graphics, clipRect, chart)
        {
            this.Font = font;
            this.Format = format;
        }
    }

    /// <summary>
    /// Provides data for TaskPaintEvent
    /// </summary>
    public class TaskPaintEventArgs : ChartPaintEventArgs
    {
        /// <summary>
        /// Get the task to be painted
        /// </summary>
        public SchedulerEvent Task { get; private set; }
        /// <summary>
        /// Get the rectangle bounds of the task
        /// </summary>
        public RectangleF Rectangle
        {
            get
            {    
                
                return new RectangleF(Chart.GetSpan(this.Task.Start - Chart.Scheduler.Start), this.Row * this.Chart.BarSpacing + this.Chart.BarSpacing + this.Chart.HeaderOneHeight, this.Chart.GetSpan(this.Task.Duration), this.Chart.BarHeight);
            }
        }
        /// <summary>
        /// Get the row number of the task
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Get or set the font to be used to draw the task label
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// Get or set the formatting of the task
        /// </summary>
        public ScheduleEventFormat Format { get; set; }
        /// <summary>
        /// Initialize a new instance of TaskPaintEventArgs with the editable default font and task paint format
        /// </summary>
        public TaskPaintEventArgs(Graphics graphics, Rectangle clipRect, SchedulerControl chart, SchedulerEvent task, int row, Font font, ScheduleEventFormat format) // need to create a paint event for each task for custom painting
            : base(graphics, clipRect, chart)
        {
            this.Task = task;
            this.Row = row;
            this.Font = font;
            this.Format = format;
        }
    }
    public class TimelinePaintEventArgs : ChartPaintEventArgs
    {
        /// <summary>
        /// Get the datetime value of the tick mark
        /// </summary>
        public DateTime DateTime { get; private set; }
        /// <summary>
        /// Get the dateimte value of the preview mark
        /// </summary>
        public DateTime DateTimePrev { get; private set; }
        /// <summary>
        /// Get or set whether painting of the tick mark has already been handled. If it is already handled, Chart will not paint the tick mark.
        /// </summary>
        public bool Handled { get; private set; }
        /// <summary>
        /// Get or set the label for the minor scale
        /// </summary>
        LabelFormat Minor { get; set; }
        /// <summary>
        /// Get or set the label for the major scale
        /// </summary>
        LabelFormat Major { get; set; }

        public TimelinePaintEventArgs(Graphics graphics, Rectangle clipRect, SchedulerControl chart, DateTime datetime, DateTime datetimeprev, LabelFormat minor, LabelFormat major)
            : base(graphics, clipRect, chart)
        {
            Handled = false;
            DateTime = datetime;
            DateTimePrev = datetimeprev;
            Minor = minor;
            Major = major;
        }
    }
}

