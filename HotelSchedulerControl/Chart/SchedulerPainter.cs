using HotelSchedulerControl.Scheduler;
using HotelSchedulerControl.ViewPort;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HotelSchedulerControl.Chart
{
    public class SchedulerPainter
    {
        HotelSchedulerControl.SchedulerControl control;
        IViewport viewport;
        SchedulerModels models;

        public SchedulerPainter(HotelSchedulerControl.SchedulerControl control, SchedulerModels models, IViewport viewport)
        {
            this.control = control;
            this.viewport = viewport;
            this.models = models;
        }

        public void PaintChart(Graphics graphics, SchedulerModels models, Rectangle clipRect)
        {
            graphics.Clear(Color.White);
            int row = 0;
            if (control.Scheduler != null)
            {
                // generate rectangles
                control.GenerateModels();
                // set model view matrix
                graphics.Transform = viewport.Projection;

                // draw columns in the background
                DrawColumns(graphics, models);
                // draw bar charts
                row = DrawScheduleEvents(graphics, models, clipRect);
                // draw the header
                DrawHeader(graphics, models, clipRect);
            }
            // flush
            graphics.Flush();
        }

        private void DrawColumns(Graphics graphics, SchedulerModels models)
        {
            // draw column lines
            graphics.DrawRectangles(control.HeaderFormat.Border, models.HeaderInfo.Columns.ToArray());

            // fill weekend columns
            for (int i = 0; i < models.HeaderInfo.DateTimes.Count; i++)
            {
                var date = models.HeaderInfo.DateTimes[i];
                // highlight weekends for day time scale
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    var pattern = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent20, control.HeaderFormat.Border.Color, Color.Transparent);
                    graphics.FillRectangle(pattern, models.HeaderInfo.Columns[i]);
                }
            }
        }

        private void DrawHeader(Graphics graphics, SchedulerModels models, Rectangle clipRect)
        {
            var info = models.HeaderInfo;
            var viewRect = viewport.Rectangle;

            // Draw header backgrounds
            var e = new HeaderPaintEventArgs(graphics, clipRect, control, control.Font, control.HeaderFormat);
            //OnPaintHeader(e);
            var gradient = new LinearGradientBrush(info.H1Rect, e.Format.GradientLight, e.Format.GradientDark, LinearGradientMode.Vertical);
            graphics.FillRectangles(gradient, new RectangleF[] { info.H1Rect, info.H2Rect });
            graphics.DrawRectangles(e.Format.Border, new RectangleF[] { info.H1Rect, info.H2Rect });

            // Draw the header scales
            DrawScale(graphics, clipRect, e.Font, e.Format, info.LabelRects, info.DateTimes);

            // draw "Now" line
            float xf = control.GetSpan(control.Scheduler.Now);
            var pen = new Pen(e.Format.Border.Color) { DashStyle = DashStyle.Dash, Color = Color.Red };
            graphics.DrawLine(pen, new PointF(xf, viewport.Y), new PointF(xf, viewport.Rectangle.Bottom));
        }

        private void DrawScale(Graphics graphics, Rectangle clipRect, Font font, HeaderFormat headerformat, List<RectangleF> labelRects, List<DateTime> dates)
        {
            TimelinePaintEventArgs e = null;
            DateTime datetime = dates[0]; // these initialisation values matter

            for (int i = 0; i < labelRects.Count; i++)
            {
                // Give user a chance to format the tickmark that is to be drawn
                // https://blog.nicholasrogoff.com/2012/05/05/c-datetime-tostring-formats-quick-reference/
                datetime = dates[i];
                GetLabel(datetime, out HeaderLabel minor, out HeaderLabel major);
                e = new TimelinePaintEventArgs(graphics, clipRect, control, datetime, new DateTime(), minor, major);
                PaintTimeline?.Invoke(this, e);

                // Draw the label if not already handled by the user
                if (!e.Handled)
                {
                    if (!string.IsNullOrEmpty(minor.Text))
                    {
                        // Draw minor label
                        var textbox = graphics.TextBoxAlign(minor.Text, minor.Format.TextAlign, minor.Format.Font, labelRects[i], minor.Format.Margin);
                        graphics.DrawString(minor.Text, minor.Format.Font, minor.Format.Color, textbox);
                    }

                    if (!string.IsNullOrEmpty(major.Text))
                    {
                        // Draw major label
                        var majorLabelRect = new RectangleF(labelRects[i].X, viewport.Y, control.MajorWidth, control.HeaderOneHeight);
                        var textbox = graphics.TextBoxAlign(major.Text, major.Format.TextAlign, major.Format.Font, majorLabelRect, major.Format.Margin);
                        graphics.DrawString(major.Text, major.Format.Font, major.Format.Color, textbox);
                        //__DrawMarker(graphics, labelRects[i].X + MinorWidth / 2f, _mViewport.Y + HeaderOneHeight - 2f);
                    }
                }
            }
        }

        private int DrawScheduleEvents(Graphics graphics, SchedulerModels models, Rectangle clipRect)
        {
            var viewRect = viewport.Rectangle;
            var pen = new Pen(Color.Gray);
            float labelMargin = control.MinorWidth / 2.0f + 3.0f;
            pen.DashStyle = DashStyle.Dot;
            TaskPaintEventArgs e;
            foreach (var task in models.EventRectangles.Keys)
            {
                // Get the taskrect
                var taskrect = models.EventRectangles[task];
                // Only begin drawing when the taskrect is to the left of the clipRect's right edge
                if (taskrect.Left <= viewRect.Right)
                {
                    e = new TaskPaintEventArgs(graphics, clipRect, control, task, task.Row, control.Font, task.Format);
                    PaintTask?.Invoke(control, e);                    

                    if (viewRect.IntersectsWith(taskrect))
                    {
                        DrawScheduleEvent(graphics, e, task, taskrect);
                    }

                    // write text
                    if (control.ShowTaskLabels && task.Name != string.Empty)
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
                    if (control.ShowSlack)
                    {
                        var slackrect = models.EventSlackRectangles[task];
                        if (viewRect.IntersectsWith(slackrect))
                            graphics.FillRectangle(e.Format.SlackFill, slackrect);
                    }
                }                
            }
            return 1;
        }
        private void DrawScheduleEvent(Graphics graphics, TaskPaintEventArgs e, ScheduleEvent task, RectangleF taskRect)
        {
            var fill = taskRect;
            fill.Width = (int)(fill.Width * 1);
            graphics.FillRectangle(e.Format.BackFill, taskRect);
            graphics.FillRectangle(e.Format.ForeFill, fill);
            graphics.DrawRectangle(e.Format.Border, taskRect);
        }
        private void GetLabel(DateTime datetime, out HeaderLabel minorLabel, out HeaderLabel majorLabel)
        {
            System.Globalization.GregorianCalendar calendar = new System.Globalization.GregorianCalendar();
            switch (control.TimeResolution)
            {
                default: // case TimeResolution.Day: -- to implement other TimeResolutions, add to this function or listen to the the PaintTimeline event
                    minorLabel = new HeaderLabel() { Text = datetime.Day.ToString(), Format = control.MinorLabelFormat };
                    majorLabel = new HeaderLabel() { Format = control.MajorLabelFormat };
                    if (datetime.Day == 15) majorLabel.Text = datetime.ToString("MMM");
                    break;
            }
        }
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
    }
}
