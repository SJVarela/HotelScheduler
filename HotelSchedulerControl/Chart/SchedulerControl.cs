﻿using HotelSchedulerControl.Chart;
using HotelSchedulerControl.Scheduler;
using HotelSchedulerControl.ViewPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace HotelSchedulerControl.Chart
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

        #endregion
        #region Methods
        #region Public
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
            _mViewport = new ControlViewport(this) { WheelDelta = BarSpacing };
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

        /// <summary>
        /// Initialize this Chart with a Project
        /// </summary>
        /// <param name="project"></param>
        public void Init(EventSchedule scheduler)
        {
            Scheduler = scheduler;
            _GenerateModels();
        }
        /// <summary>
        /// Convert the specified timespan to pixels units of the Chart x-coordinates
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert the pixel units of the Chart x-coordinates to TimeSpan
        /// </summary>
        /// <param name="dx"></param>
        /// <returns></returns>
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
        #endregion
        #region Private

        protected virtual void OnPaintHeader(HeaderPaintEventArgs e)
        {
            PaintHeader?.Invoke(this, e);
        }

        private void PaintChart(Graphics graphics, Rectangle clipRect)
        {
            graphics.Clear(Color.White);
            int row = 0;
            if (Scheduler != null)
            {
                // generate rectangles
                _GenerateModels();
                _GenerateHeaders();

                // set model view matrix
                graphics.Transform = _mViewport.Projection;

                // draw columns in the background
                _DrawColumns(graphics);
                // draw bar charts
                row = this._DrawTasks(graphics, clipRect);
                // draw the header
                DrawHeader(graphics, clipRect);
            }
            // flush
            graphics.Flush();
        }
        private void DrawHeader(Graphics graphics, Rectangle clipRect)
        {
            var info = _mHeaderInfo;
            var viewRect = _mViewport.Rectangle;

            // Draw header backgrounds
            var e = new HeaderPaintEventArgs(graphics, clipRect, this, this.Font, this.HeaderFormat);
            OnPaintHeader(e);
            var gradient = new LinearGradientBrush(info.H1Rect, e.Format.GradientLight, e.Format.GradientDark, LinearGradientMode.Vertical);
            graphics.FillRectangles(gradient, new RectangleF[] { info.H1Rect, info.H2Rect });
            graphics.DrawRectangles(e.Format.Border, new RectangleF[] { info.H1Rect, info.H2Rect });

            // Draw the header scales
            __DrawScale(graphics, clipRect, e.Font, e.Format, info.LabelRects, info.DateTimes);

            // draw "Now" line
            float xf = GetSpan(Scheduler.Now);
            var pen = new Pen(e.Format.Border.Color) { DashStyle = DashStyle.Dash, Color = Color.Red };
            graphics.DrawLine(pen, new PointF(xf, _mViewport.Y), new PointF(xf, _mViewport.Rectangle.Bottom));
        }
        private void ___GetLabelFormat(DateTime datetime, DateTime datetimeprev, out LabelFormat minor, out LabelFormat major)
        {
            minor = new LabelFormat() { Text = string.Empty, Font = this.Font, Color = HeaderFormat.Color, Margin = 3, TextAlign = ChartTextAlign.MiddleCenter };
            major = new LabelFormat() { Text = string.Empty, Font = this.Font, Color = HeaderFormat.Color, Margin = 3, TextAlign = ChartTextAlign.MiddleLeft };

            System.Globalization.GregorianCalendar calendar = new System.Globalization.GregorianCalendar();
            switch (TimeResolution)
            {
                case TimeResolution.Week:
                    minor.Text = calendar.GetWeekOfYear(datetime, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString();
                    if (datetime.Month != datetimeprev.Month) major.Text = datetime.ToString("MMMM");
                    break;
                //case TimeResolution.Hour:
                //    minor.Text = datetime.Hour.ToString();
                //    if (datetime.Day != datetimeprev.Day) major.Text = datetime.ToString("dd MMM yyyy");
                //    break;
                default: // case TimeResolution.Day: -- to implement other TimeResolutions, add to this function or listen to the the PaintTimeline event
                    minor.Text = datetime.Day.ToString(); // datetime.ToString("dddd").Substring(0, 1).ToUpper();
                    if (datetime.Day == 15) major.Text = datetime.ToString("MMM");
                    break;
            }
        }
        private void __DrawMarker(Graphics graphics, float offsetX, float offsetY)
        {
            //var marker = _Marker.Select(p => new PointF(p.X + offsetX, p.Y + offsetY)).ToArray();
            //graphics.FillPolygon(Brushes.LightGoldenrodYellow, marker);
            //graphics.DrawPolygon(new Pen(SystemColors.ButtonShadow), marker);
        }
        private void __DrawScale(Graphics graphics, Rectangle clipRect, Font font, HeaderFormat headerformat, List<RectangleF> labelRects, List<DateTime> dates)
        {
            TimelinePaintEventArgs e = null;
            DateTime datetime = dates[0]; // these initialisation values matter
            DateTime datetimeprev = dates[0]; // these initialisation values matter
            for (int i = 0; i < labelRects.Count; i++)
            {
                // Give user a chance to format the tickmark that is to be drawn
                // https://blog.nicholasrogoff.com/2012/05/05/c-datetime-tostring-formats-quick-reference/
                datetime = dates[i];
                ___GetLabelFormat(datetime, datetimeprev, out LabelFormat minor, out LabelFormat major);
                //e = new TimelinePaintEventArgs(graphics, clipRect, this, datetime, datetimeprev, minor, major);
                OnPaintTimeline(e);

                // Draw the label if not already handled by the user
                if (!e.Handled)
                {
                    if (!string.IsNullOrEmpty(minor.Text))
                    {
                        // Draw minor label
                        var textbox = graphics.TextBoxAlign(minor.Text, minor.TextAlign, minor.Font, labelRects[i], minor.Margin);
                        graphics.DrawString(minor.Text, minor.Font, minor.Color, textbox);
                    }

                    if (!string.IsNullOrEmpty(major.Text))
                    {
                        // Draw major label
                        var majorLabelRect = new RectangleF(labelRects[i].X, _mViewport.Y, this.MajorWidth, this.HeaderOneHeight);
                        var textbox = graphics.TextBoxAlign(major.Text, major.TextAlign, major.Font, majorLabelRect, major.Margin);
                        graphics.DrawString(major.Text, major.Font, major.Color, textbox);
                        __DrawMarker(graphics, labelRects[i].X + MinorWidth / 2f, _mViewport.Y + HeaderOneHeight - 2f);
                    }
                }

                // set prev datetime
                datetimeprev = datetime;
            }
        }
        private void _DrawColumns(Graphics graphics)
        {
            // draw column lines
            graphics.DrawRectangles(this.HeaderFormat.Border, _mHeaderInfo.Columns.ToArray());

            // fill weekend columns
            for (int i = 0; i < _mHeaderInfo.DateTimes.Count; i++)
            {
                var date = _mHeaderInfo.DateTimes[i];
                // highlight weekends for day time scale
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    var pattern = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent20, this.HeaderFormat.Border.Color, Color.Transparent);
                    graphics.FillRectangle(pattern, _mHeaderInfo.Columns[i]);
                }
            }
        }
        private void __DrawTaskParts(Graphics graphics, TaskPaintEventArgs e, ScheduleEvent task, Pen pen)
        {
            var parts = _mChartTaskPartRects[task];

            // Draw line indicator
            var firstRect = parts[0].Value;
            var lastRect = parts[parts.Count - 1].Value;
            var y_coord = (firstRect.Top + firstRect.Bottom) / 2.0f;
            var point1 = new PointF(firstRect.Right, y_coord);
            var point2 = new PointF(lastRect.Left, y_coord);
            graphics.DrawLine(pen, point1, point2);

            // Draw Part Rectangles
            var taskRects = parts.Select(x => x.Value).ToArray();
            graphics.FillRectangles(e.Format.BackFill, taskRects);

            // Draw % complete indicators
            graphics.FillRectangles(e.Format.ForeFill, parts.Select(x => new RectangleF(x.Value.X, x.Value.Y, x.Value.Width * 1, x.Value.Height)).ToArray());

            // Draw border
            graphics.DrawRectangles(e.Format.Border, taskRects);
        }
        private void __DrawRegularTaskAndGroup(Graphics graphics, TaskPaintEventArgs e, ScheduleEvent task, RectangleF taskRect)
        {
            var fill = taskRect;
            fill.Width = (int)(fill.Width * 1);
            graphics.FillRectangle(e.Format.BackFill, taskRect);
            graphics.FillRectangle(e.Format.ForeFill, fill);
            graphics.DrawRectangle(e.Format.Border, taskRect);
        }
        private int _DrawTasks(Graphics graphics, Rectangle clipRect)
        {
            var viewRect = _mViewport.Rectangle;
            int row = 0;
            var pen = new Pen(Color.Gray);
            float labelMargin = this.MinorWidth / 2.0f + 3.0f;
            pen.DashStyle = DashStyle.Dot;
            TaskPaintEventArgs e;
            foreach (var task in _mChartTaskRects.Keys)
            {
                // Get the taskrect
                var taskrect = _mChartTaskRects[task];
                // Only begin drawing when the taskrect is to the left of the clipRect's right edge
                if (taskrect.Left <= viewRect.Right)
                {
                    e = new TaskPaintEventArgs(graphics, clipRect, this, task, row, this.Font, task.Format);
                    PaintTask?.Invoke(this, e);

                    if (viewRect.IntersectsWith(taskrect))
                    {
                        __DrawRegularTaskAndGroup(graphics, e, task, taskrect);
                    }

                    // write text
                    if (this.ShowTaskLabels && task.Name != string.Empty)
                    {
                        var name = task.Name;
                        var txtrect = graphics.TextBoxAlign(name, ChartTextAlign.MiddleLeft, e.Font, taskrect, labelMargin);
                        txtrect.Offset(taskrect.Width, 0);
                        if (viewRect.IntersectsWith(txtrect))
                        {
                            graphics.DrawString(name, e.Font, e.Format.Color, txtrect);
                        }
                    }

                    // draw slack
                    if (this.ShowSlack)
                    {
                        var slackrect = _mChartSlackRects[task];
                        if (viewRect.IntersectsWith(slackrect))
                            graphics.FillRectangle(e.Format.SlackFill, slackrect);
                    }
                }
                row++;
            }
            return row;
        }
        /// <summary>
        /// Generate the task models and resize the world accordingly
        /// </summary>
        private void _GenerateModels()
        {
            // Clear Models
            _mChartTaskRects.Clear();
            _mChartTaskHitRects.Clear();
            _mChartSlackRects.Clear();
            _mChartTaskPartRects.Clear();

            var pHeight = this.Parent == null ? this.Height : this.Parent.Height;
            var pWidth = this.Parent == null ? this.Width : this.Parent.Width;

            // loop over the tasks and pick up items
            var end = TimeSpan.MinValue;

            foreach (var task in Scheduler.Tasks)
            {
                if (true)
                {
                    int y_coord = task.Row * this.BarSpacing + this.HeaderTwoHeight + this.HeaderOneHeight + (this.BarSpacing - this.BarHeight) / 2;
                    RectangleF taskRect;

                    // Compute task rectangle
                    taskRect = new RectangleF(GetSpan(task.Start - Scheduler.Start), y_coord, GetSpan(task.Duration), this.BarHeight);
                    _mChartTaskRects.Add(task, taskRect); // also add groups and split tasks (not just task parts)
                    _mChartTaskHitRects.Add(task, taskRect);
                    // Compute Slack Rectangles
                    if (this.ShowSlack)
                    {
                        var slackRect = new RectangleF(GetSpan(task.End - Scheduler.Start), y_coord, GetSpan(task.Slack), this.BarHeight);
                        _mChartSlackRects.Add(task, slackRect);
                    }
                    // Find maximum end time
                    if ((task.End - Scheduler.Start) > end) end = task.End - Scheduler.Start;

                }
            }
            //row += 5;
            _mViewport.WorldHeight = Math.Max(pHeight, this.BarSpacing + this.BarHeight);
            _mViewport.WorldWidth = Math.Max(pWidth, GetSpan(end) + 200);
        }
        /// <summary>
        /// Generate Header rectangles and dates
        /// </summary>
        private void _GenerateHeaders()
        {
            // only generate the necessary headers by determining the current viewport location
            var h1Rect = new RectangleF(_mViewport.X, _mViewport.Y, _mViewport.Rectangle.Width, this.HeaderOneHeight);
            var h2Rect = new RectangleF(h1Rect.Left, h1Rect.Bottom, _mViewport.Rectangle.Width, this.HeaderTwoHeight);
            var labelRects = new List<RectangleF>();
            var columns = new List<RectangleF>();
            var datetimes = new List<DateTime>();

            // generate columns across the viewport area           
            var minorDate = __CalculateViewportStart(); // start date of chart
            var minorInterval = GetSpan(MinorWidth);
            // calculate coordinates of rectangles
            var labelRect_Y = _mViewport.Y + this.HeaderOneHeight;
            var labelRect_X = (int)(_mViewport.X / MinorWidth) * MinorWidth;
            var columns_Y = labelRect_Y + this.HeaderTwoHeight;

            // From second column onwards,
            // loop over the number of <TimeScaleDisplay> each with width of MajorWidth,
            // creating the Major and Minor header rects and generating respective date time information
            while (labelRect_X < _mViewport.Rectangle.Right) // keep creating H1 labels until we are out of the viewport
            {
                datetimes.Add(minorDate);
                labelRects.Add(new RectangleF(labelRect_X, labelRect_Y, MinorWidth, HeaderTwoHeight));
                columns.Add(new RectangleF(labelRect_X, columns_Y, MinorWidth, _mViewport.Rectangle.Height));
                minorDate += minorInterval;
                labelRect_X += MinorWidth;
            }

            _mHeaderInfo.H1Rect = h1Rect;
            _mHeaderInfo.H2Rect = h2Rect;
            _mHeaderInfo.LabelRects = labelRects;
            _mHeaderInfo.Columns = columns;
            _mHeaderInfo.DateTimes = datetimes;
        }

        private DateTime __CalculateViewportStart()
        {
            float vpTime = (int)(_mViewport.X / this.MinorWidth);
            if (this.TimeResolution == TimeResolution.Week)
            {
                return Scheduler.Start.AddDays(vpTime * 7);
            }
            else if (this.TimeResolution == TimeResolution.Day)
            {
                return Scheduler.Start.AddDays(vpTime);
            }
            throw new NotImplementedException("Unable to determine TimeResolution.");
        }

        #region Private Helper Variables'

        /// <summary>
        /// Polygon points for Header markers
        /// </summary>
        private static readonly PointF[] _Marker = new PointF[] {
            new PointF(-4, 0),
            new PointF(4, 0),
            new PointF(4, 4),
            new PointF(0, 8),
            new PointF(-4f, 4)
        };

        IViewport _mViewport = null;
        ScheduleEvent _mDraggedTask = null; // The dragged source Task
        Point _mDragTaskLastLocation = Point.Empty; // Record the task dragging mouse offset
        Point _mDragTaskStartLocation = Point.Empty;
        Point _mPanViewLastLocation = Point.Empty;
        List<ScheduleEvent> _mSelectedTasks = new List<ScheduleEvent>(); // List of selected tasks
        Dictionary<ScheduleEvent, RectangleF> _mChartTaskHitRects = new Dictionary<ScheduleEvent, RectangleF>(); // list of hitareas for Task Rectangles
        Dictionary<ScheduleEvent, RectangleF> _mChartTaskRects = new Dictionary<ScheduleEvent, RectangleF>();
        Dictionary<ScheduleEvent, List<KeyValuePair<ScheduleEvent, RectangleF>>> _mChartTaskPartRects = new Dictionary<ScheduleEvent, List<KeyValuePair<ScheduleEvent, RectangleF>>>();
        Dictionary<ScheduleEvent, RectangleF> _mChartSlackRects = new Dictionary<ScheduleEvent, RectangleF>();
        HeaderInfo _mHeaderInfo = new HeaderInfo();
        ScheduleEvent _mMouseEntered = null; // flag whether the mouse has entered a Task rectangle or not
        Dictionary<ScheduleEvent, string> _mTaskToolTip = new Dictionary<ScheduleEvent, string>();
        #endregion Private Helper Variables
        #endregion

        #endregion
        #region Events

        /// <summary>
        /// Raises the System.Windows.Forms.Control.Paint event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!this.DesignMode)
                this.PaintChart(e.Graphics, e.ClipRectangle);
        }
        protected virtual void OnPaintTimeline(TimelinePaintEventArgs e)
        {
            PaintTimeline?.Invoke(this, e);
        }
        /// <summary>
        /// Occurs when the mouse is moving over a Task
        /// </summary>
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

        /// <summary>
        /// Occurs before a Task gets painted
        /// </summary>
        public event EventHandler<TaskPaintEventArgs> PaintTask = null;

        /// <summary>
        /// Occurs before overlays get painted
        /// </summary>
        public event EventHandler<ChartPaintEventArgs> PaintOverlay = null;

        /// <summary>
        /// Occurs before the header gets painted
        /// </summary>
        public event EventHandler<HeaderPaintEventArgs> PaintHeader = null;

        /// <summary>
        /// Occurs before the header date tick mark gets painted
        /// </summary>
        public event EventHandler<TimelinePaintEventArgs> PaintTimeline = null;
        #endregion
    }
    #region ChartFormat
    public struct ChartInfo
    {
        /// <summary>
        /// Get or set the chart row number
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// Get or set the chart date/time
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Get or set the task
        /// </summary>
        public ScheduleEvent Task { get; set; }
        /// <summary>
        /// Construct a passive data structure to hold chart information
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dateTime"></param>
        /// <param name="task"></param>
        public ChartInfo(int row, DateTime dateTime, ScheduleEvent task)
            : this()
        {
            Row = row;
            DateTime = dateTime;
            Task = task;
        }
    }

    public class Row
    {
        public int Index { get; set; }
        public float Height { get; set; }
    }

    public class Column
    {
        public int Index { get; set; }
        public DateTime DateTime { get; set; }
    }
    //public enum ChartTextAlign
    //{
    //    TopLeft, TopCenter, TopRight,
    //    MiddleLeft, MiddleCenter, MiddleRight,
    //    BottomLeft, BottomCenter, BottomRight
    //}
    public class LabelFormat
    {
        public string Text;
        public Font Font;
        public Brush Color;
        public ChartTextAlign TextAlign;
        public float Margin;
    }
    #endregion
    //public enum TimeResolution
    //{
    //    Day,
    //    Week,
    //    Month
    //}

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
}

