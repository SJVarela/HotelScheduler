using HotelSchedulerControl.Chart;
using HotelSchedulerControl.Scheduler;
using HotelSchedulerControl.ViewPort;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelSchedulerControl
{
    public partial class SchedulerControl : UserControl
    {
        #region Properties
        [DefaultValue(32)]
        public int HeaderOneHeight { get; set; }
        [DefaultValue(20)]
        public int HeaderTwoHeight { get; set; }
        [DefaultValue(32)]
        public int BarSpacing { get; set; }
        [DefaultValue(20)]
        public int BarHeight { get; set; }
        [DefaultValue(TimeResolution.Day)]
        public TimeResolution TimeResolution { get; set; }
        [DefaultValue(20)]
        public int MinorWidth { get; set; }
        [DefaultValue(140)]
        public int MajorWidth { get; set; }
        public HeaderLabelFormat MajorLabelFormat { get; set; }
        public HeaderLabelFormat MinorLabelFormat { get; set; }
        public HeaderFormat HeaderFormat { get; set; }
        [DefaultValue(true)]
        public bool ShowTaskLabels { get; set; }
        [DefaultValue(true)]
        public bool ShowSlack { get; set; }

        public EventSchedule Scheduler = null;

        private SchedulerModelGenerator modelGenerator;
        private SchedulerPainter painter;
        private IViewport viewPort;

        private SchedulerModels models = new SchedulerModels();

        #endregion

        public SchedulerControl()
        {
            InitializeComponent();

            HeaderOneHeight = 32;
            HeaderTwoHeight = 20;
            BarSpacing = 32;
            BarHeight = 20;
            MajorWidth = 140;
            MinorWidth = 20;
            TimeResolution = TimeResolution.Day;
            this.DoubleBuffered = true;

            viewPort = new ControlViewport(this) { WheelDelta = BarSpacing };
            modelGenerator = new SchedulerModelGenerator(this, viewPort, models);
            painter = new SchedulerPainter(this, models, viewPort);
            //AllowTaskDragDrop = true;
            ShowSlack = true;
            ShowTaskLabels = true;
            this.Dock = DockStyle.Fill;
            this.Margin = new Padding(0, 0, 0, 0);
            this.Padding = new Padding(0, 0, 0, 0);
            HeaderFormat = new HeaderFormat()
            {
                Color = Brushes.Black,
                Border = new Pen(SystemColors.ActiveBorder),
                GradientLight = SystemColors.ButtonHighlight,
                GradientDark = SystemColors.ButtonFace
            };
            MajorLabelFormat = new HeaderLabelFormat()
            {
                Font = Font,
                Color = HeaderFormat.Color,
                Margin = 3,
                TextAlign = ChartTextAlign.MiddleCenter
            };
            MinorLabelFormat = new HeaderLabelFormat()
            {
                Font = Font,
                Color = HeaderFormat.Color,
                Margin = 3,
                TextAlign = ChartTextAlign.MiddleCenter
            };
        }
        public void Init(EventSchedule scheduler)
        {
            Scheduler = scheduler;
            GenerateModels();
        }
        public float GetSpan(TimeSpan span)
        {
            double pixels = 0;
            switch (TimeResolution)
            {
                case TimeResolution.Day:
                    pixels = span.TotalDays * (double)MinorWidth;
                    break;
                case TimeResolution.Week:
                    pixels = span.TotalDays / 7f * (double)MinorWidth;
                    break;
            }
            return (float)pixels;
        }
        public TimeSpan GetSpan(float dx)
        {
            TimeSpan span = TimeSpan.MinValue;
            switch (TimeResolution)
            {
                case TimeResolution.Day:
                    span = TimeSpan.FromDays(dx / MinorWidth);
                    break;
                case TimeResolution.Week:
                    span = TimeSpan.FromDays(dx / MinorWidth * 7f);
                    break;
            }
            return span;
        }

        public void GenerateModels()
        {
            modelGenerator.GenerateEventModels();
            modelGenerator.GenerateHeaders();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this.DesignMode)
                painter.PaintChart(e.Graphics, models, e.ClipRectangle);
        }

        public event EventHandler<TaskMouseEventArgs> TaskMouseOver = null;

        /// <summary>
        /// Occurs when the mouse leaves a Task
        /// </summary>
        public event EventHandler<TaskMouseEventArgs> TaskMouseOut = null;

        /// <summary>
        /// Occurs when a Task is clicked
        /// </summary>
        public event EventHandler<TaskMouseEventArgs> TaskMouseClick = null;

        /// <summary>
        /// Occurs when a Task is double clicked by the mouse
        /// </summary>
        public event EventHandler<TaskMouseEventArgs> TaskMouseDoubleClick = null;

        /// <summary>
        /// Occurs when a Task is being dragged by the mouse
        /// </summary>
        public event EventHandler<TaskDragDropEventArgs> TaskMouseDrag = null;

        /// <summary>
        /// Occurs when a dragged Task is being dropped by releasing any previously pressed mouse button.
        /// </summary>
        public event EventHandler<TaskDragDropEventArgs> TaskMouseDrop = null;

        /// <summary>
        /// Occurs when a task is selected.
        /// </summary>
        public event EventHandler<TaskMouseEventArgs> TaskSelected = null;

        /// <summary>
        /// Occurs before one or more tasks are being deselected. All Task in Chart.SelectedTasks will be deselected.
        /// </summary>
        public event EventHandler<TaskMouseEventArgs> TaskDeselecting = null;
       
    }

    static class GDIExtention
    {
        public static void DrawRectangle(this Graphics graphics, Pen pen, RectangleF rectangle)
        {
            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static RectangleF TextBoxAlign(this Graphics graphics, string text, ChartTextAlign align, Font font, RectangleF textbox, float margin = 0)
        {
            var size = graphics.MeasureString(text, font);
            switch (align)
            {
                case ChartTextAlign.MiddleCenter:
                    return new RectangleF(new PointF(textbox.Left + (textbox.Width - size.Width) / 2, textbox.Top + (textbox.Height - size.Height) / 2), size);
                case ChartTextAlign.MiddleLeft:
                    return new RectangleF(new PointF(textbox.Left + margin, textbox.Top + (textbox.Height - size.Height) / 2), size);
                default:
                    return new RectangleF(new PointF(textbox.Left + (textbox.Width - size.Width) / 2, textbox.Top + (textbox.Height - size.Height) / 2), size);

            }
        }
    }
    public enum TimeResolution
    {
        Day,
        Week,
        Month
    }
    public enum ChartTextAlign
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

}
