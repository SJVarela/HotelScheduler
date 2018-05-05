using HotelSchedulerControl.Scheduler;
using HotelSchedulerControl.ViewPort;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HotelSchedulerControl.Chart
{
    public class SchedulerModelGenerator
    {
        private IViewport viewport;
        private HotelSchedulerControl.SchedulerControl control;
        SchedulerModels models;

        public SchedulerModelGenerator(HotelSchedulerControl.SchedulerControl control, IViewport viewport, SchedulerModels models)
        {
            this.viewport = viewport;
            this.control = control;
            this.models = models;
        }
        public void GenerateEventModels()
        {
            // Clear Models
            models.EventRectangles.Clear();
            models.EventHitRectangle.Clear();
            models.EventSlackRectangles.Clear();
            models.EventPartsRectangle.Clear();

            var pHeight = control.Parent == null ? control.Height : control.Parent.Height;
            var pWidth = control.Parent == null ? control.Width : control.Parent.Width;

            // loop over the tasks and pick up items
            var end = TimeSpan.MinValue;

            foreach (var task in control.Scheduler.Tasks)
            {
                if (true)
                {
                    int y_coord = task.Row * control.BarSpacing + control.HeaderTwoHeight + control.HeaderOneHeight + (control.BarSpacing - control.BarHeight) / 2;
                    RectangleF taskRect;

                    // Compute task rectangle
                    taskRect = new RectangleF(control.GetSpan(task.Start - control.Scheduler.Start), y_coord, control.GetSpan(task.Duration), control.BarHeight);
                    models.EventRectangles.Add(task, taskRect); // also add groups and split tasks (not just task parts)
                    models.EventHitRectangle.Add(task, taskRect);
                    // Compute Slack Rectangles
                    if (control.ShowSlack)
                    {
                        var slackRect = new RectangleF(control.GetSpan(task.End - control.Scheduler.Start), y_coord, control.GetSpan(task.Slack), control.BarHeight);
                        models.EventSlackRectangles.Add(task, slackRect);
                    }
                    // Find maximum end time
                    if ((task.End - control.Scheduler.Start) > end) end = task.End - control.Scheduler.Start;

                }
            }
            //row += 5;
            viewport.WorldHeight = Math.Max(pHeight, control.BarSpacing + control.BarHeight);
            viewport.WorldWidth = Math.Max(pWidth, control.GetSpan(end) + 200);
        }

        public void GenerateHeaders()
        {
            // only generate the necessary headers by determining the current viewport location
            var h1Rect = new RectangleF(viewport.X, viewport.Y, viewport.Rectangle.Width, control.HeaderOneHeight);
            var h2Rect = new RectangleF(h1Rect.Left, h1Rect.Bottom, viewport.Rectangle.Width, control.HeaderTwoHeight);
            var labelRects = new List<RectangleF>();
            var columns = new List<RectangleF>();
            var datetimes = new List<DateTime>();

            // generate columns across the viewport area           
            var minorDate = CalculateViewportStart(); // start date of chart
            var minorInterval = control.GetSpan(control.MinorWidth);
            // calculate coordinates of rectangles
            var labelRect_Y = viewport.Y + control.HeaderOneHeight;
            var labelRect_X = (int)(viewport.X / control.MinorWidth) * control.MinorWidth;
            var columns_Y = labelRect_Y + control.HeaderTwoHeight;

            // From second column onwards,
            // loop over the number of <TimeScaleDisplay> each with width of MajorWidth,
            // creating the Major and Minor header rects and generating respective date time information
            while (labelRect_X < viewport.Rectangle.Right) // keep creating H1 labels until we are out of the viewport
            {
                datetimes.Add(minorDate);
                labelRects.Add(new RectangleF(labelRect_X, labelRect_Y, control.MinorWidth, control.HeaderTwoHeight));
                columns.Add(new RectangleF(labelRect_X, columns_Y, control.MinorWidth, viewport.Rectangle.Height));
                minorDate += minorInterval;
                labelRect_X += control.MinorWidth;
            }

            models.HeaderInfo.H1Rect = h1Rect;
            models.HeaderInfo.H2Rect = h2Rect;
            models.HeaderInfo.LabelRects = labelRects;
            models.HeaderInfo.Columns = columns;
            models.HeaderInfo.DateTimes = datetimes;
        }

        private DateTime CalculateViewportStart()
        {
            float vpTime = (int)(viewport.X / control.MinorWidth);
            if (control.TimeResolution == TimeResolution.Week)
            {
                return control.Scheduler.Start.AddDays(vpTime * 7);
            }
            else if (control.TimeResolution == TimeResolution.Day)
            {
                return control.Scheduler.Start.AddDays(vpTime);
            }
            throw new NotImplementedException("Unable to determine TimeResolution.");
        }

    }
}
