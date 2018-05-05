using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HotelSchedulerControl.Scheduler
{
    public class ScheduleEvent
    {
        public ScheduleEvent(int row)
        {
            Start = DateTime.Now;
            End = Start;
            Slack = TimeSpan.Zero;
            Row = row;
        }

        public string Name { get; set; }
        public int Row { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Duration { get { return End - Start; } }
        public TimeSpan Slack { get; set; }
        public ScheduleEventFormat Format { get; set; } = new ScheduleEventFormat()
        {
            Color = Brushes.Black,
            Border = Pens.Maroon,
            BackFill = Brushes.MediumSlateBlue,
            ForeFill = Brushes.YellowGreen,
            SlackFill = new HatchBrush(HatchStyle.LightDownwardDiagonal, Color.Blue, Color.Transparent)
        };

        public override string ToString()
        {
            return string.Format("[Name = {0}, Start = {1}, End = {2}, Duration = {3}]", Name, Start, End, Duration);
        }
    }
}
