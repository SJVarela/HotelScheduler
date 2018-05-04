using HotelSchedulerControl.ProyectManager;
using HotelSchedulerControl.ViewPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HotelSchedulerControl.Chart
{
    public enum TimeResolution
    {
        Day,
        Week,
        Month
    }
    public partial class SchedulerControl : UserControl
    {

        #region Properties
        /// <summary>
        /// Get or set header1 pixel height
        /// </summary>
        [DefaultValue(32)]
        public int HeaderOneHeight { get; set; }

        /// <summary>
        /// Get or set header2 pixel height
        /// </summary>
        [DefaultValue(20)]
        public int HeaderTwoHeight { get; set; }

        /// <summary>
        /// Get or set pixel distance from top of each Task to the next
        /// </summary>
        [DefaultValue(32)]
        public int BarSpacing { get; set; }

        /// <summary>
        /// Get or set pixel height of each Task
        /// </summary>
        [DefaultValue(20)]
        public int BarHeight { get; set; }

        /// <summary>
        /// Get or set the time scale display format
        /// </summary>
        [DefaultValue(TimeResolution.Day)]
        public TimeResolution TimeResolution { get; set; }

        /// <summary>
        /// Get or set the pixel width of each step of the time scale e.g. if TimeScale is TimeScale.Day, then each Day will be TimeWidth pixels apart
        /// </summary>
        [DefaultValue(20)]
        public int MinorWidth { get; set; }

        /// <summary>
        /// Get or set pixel width between major tick marks.
        /// </summary>
        [DefaultValue(140)]
        public int MajorWidth { get; set; }

        ///// <summary>
        ///// Get or set format for Tasks
        ///// </summary>
        //public TaskFormat TaskFormat { get; set; }
        /// <summary>
        /// Get or set format for headers
        /// </summary>
        public HeaderFormat HeaderFormat { get; set; }
        /// <summary>
        /// Get or set whether to show task labels
        /// </summary>
        [DefaultValue(true)]
        public bool ShowTaskLabels { get; set; }
        /// <summary>
        /// Get or set whether to show slack
        /// </summary>
        [DefaultValue(false)]
        public bool ShowSlack { get; set; }
        #endregion
        #region Methods

        #region Public
        public SchedulerControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initialize this Chart with a Project
        /// </summary>
        /// <param name="project"></param>
        public void Init(ProjectManager<Task, object> project)
        {
            _mProject = project;
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
        #endregion
        #region Private
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
            int row = 0;
            foreach (var task in _mProject.Tasks)
            {
                if (!_mProject.GroupsOf(task).Any(x => x.IsCollapsed))
                {
                    int y_coord = row * this.BarSpacing + this.HeaderTwoHeight + this.HeaderOneHeight + (this.BarSpacing - this.BarHeight) / 2;
                    RectangleF taskRect;

                    // Compute task rectangle
                    taskRect = new RectangleF(GetSpan(task.Start), y_coord, GetSpan(task.Duration), this.BarHeight);
                    _mChartTaskRects.Add(task, taskRect); // also add groups and split tasks (not just task parts)

                    if (!_mProject.IsSplit(task))
                    {
                        // Add normal Task Rectangles to hitRect collection for hit testing
                        _mChartTaskHitRects.Add(task, taskRect);
                    }
                    else // Compute task part rectangles if task is a split task
                    {
                        var parts = new List<KeyValuePair<Task, RectangleF>>();
                        _mChartTaskPartRects.Add(task, parts);
                        foreach (var part in _mProject.PartsOf(task))
                        {
                            taskRect = new RectangleF(GetSpan(part.Start), y_coord, GetSpan(part.Duration), this.BarHeight);
                            parts.Add(new KeyValuePair<Task, RectangleF>(part, taskRect));

                            // Parts are mouse enabled, add to hitRect collection
                            _mChartTaskHitRects.Add(part, taskRect);
                        }
                    }

                    // Compute Slack Rectangles
                    if (this.ShowSlack)
                    {
                        var slackRect = new RectangleF(GetSpan(task.End), y_coord, GetSpan(task.Slack), this.BarHeight);
                        _mChartSlackRects.Add(task, slackRect);
                    }

                    // Find maximum end time
                    if (task.End > end) end = task.End;

                    row++;
                }
            }
            row += 5;
            _mViewport.WorldHeight = Math.Max(pHeight, row * this.BarSpacing + this.BarHeight);
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
                return _mProject.Start.AddDays(vpTime * 7);
            }
            else if (this.TimeResolution == TimeResolution.Day)
            {
                return _mProject.Start.AddDays(vpTime);
            }
            throw new NotImplementedException("Unable to determine TimeResolution.");
        }

        #region Private Helper Variables'
        /// <summary>
        /// Printing labels for header
        /// </summary>
        private static readonly SortedDictionary<DayOfWeek, string> ShortDays = new SortedDictionary<DayOfWeek, string>
        {
            {DayOfWeek.Sunday, "S"},
            {DayOfWeek.Monday, "M"},
            {DayOfWeek.Tuesday, "T"},
            {DayOfWeek.Wednesday, "W"},
            {DayOfWeek.Thursday, "T"},
            {DayOfWeek.Friday, "F"},
            {DayOfWeek.Saturday, "S"}
        };

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

        class HeaderInfo
        {
            public RectangleF H1Rect;
            public RectangleF H2Rect;
            public List<RectangleF> LabelRects;
            public List<RectangleF> Columns;
            public List<DateTime> DateTimes;
        }

        ProjectManager<Task, object> _mProject = null; // The project to be visualised / rendered as a Gantt Chart
        IViewport _mViewport = null;
        Task _mDraggedTask = null; // The dragged source Task
        Point _mDragTaskLastLocation = Point.Empty; // Record the task dragging mouse offset
        Point _mDragTaskStartLocation = Point.Empty;
        Point _mPanViewLastLocation = Point.Empty;
        List<Task> _mSelectedTasks = new List<Task>(); // List of selected tasks
        Dictionary<Task, RectangleF> _mChartTaskHitRects = new Dictionary<Task, RectangleF>(); // list of hitareas for Task Rectangles
        Dictionary<Task, RectangleF> _mChartTaskRects = new Dictionary<Task, RectangleF>();
        Dictionary<Task, List<KeyValuePair<Task, RectangleF>>> _mChartTaskPartRects = new Dictionary<Task, List<KeyValuePair<Task, RectangleF>>>();
        Dictionary<Task, RectangleF> _mChartSlackRects = new Dictionary<Task, RectangleF>();
        HeaderInfo _mHeaderInfo = new HeaderInfo();
        Task _mMouseEntered = null; // flag whether the mouse has entered a Task rectangle or not
        Dictionary<Task, string> _mTaskToolTip = new Dictionary<Task, string>();
        #endregion Private Helper Variables
        #endregion

        #endregion
        #region Events
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
        public Task Task { get; set; }
        /// <summary>
        /// Construct a passive data structure to hold chart information
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dateTime"></param>
        /// <param name="task"></param>
        public ChartInfo(int row, DateTime dateTime, Task task)
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
    public enum ChartTextAlign
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }
    public class LabelFormat
    {
        public string Text;
        public Font Font;
        public Brush Color;
        public ChartTextAlign TextAlign;
        public float Margin;
    }
    #endregion

}

