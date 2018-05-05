using HotelSchedulerControl.Scheduler;
using System.Collections.Generic;
using System.Drawing;

namespace HotelSchedulerControl.Chart
{
    public class SchedulerModels
    {
        public Dictionary<ScheduleEvent, RectangleF> EventRectangles { get; set; } = new Dictionary<ScheduleEvent, RectangleF>();
        public Dictionary<ScheduleEvent, RectangleF> EventHitRectangle { get; set; } = new Dictionary<ScheduleEvent, RectangleF>();
        public Dictionary<ScheduleEvent, List<KeyValuePair<ScheduleEvent, RectangleF>>> EventPartsRectangle { get; set; } = new Dictionary<ScheduleEvent, List<KeyValuePair<ScheduleEvent, RectangleF>>>();
        public Dictionary<ScheduleEvent, RectangleF> EventSlackRectangles { get; set; } = new Dictionary<ScheduleEvent, RectangleF>();

        public HeaderInfo HeaderInfo { get; set; } = new HeaderInfo();
    }
}
