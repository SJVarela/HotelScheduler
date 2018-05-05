using System.Drawing;

namespace HotelSchedulerControl.Chart
{
    public class HeaderFormat
    {
        public Brush Color { get; set; }
        public Pen Border { get; set; }
        public Color GradientLight { get; set; }
        public Color GradientDark { get; set; }
    }    
    public class HeaderLabelFormat
    {
        public Font Font { get; set; }
        public Brush Color { get; set; }
        public ChartTextAlign TextAlign { get; set; }
        public float Margin { get; set; }
    }
    public class HeaderLabel
    {        
        public string Text { get; set; }
        public HeaderLabelFormat Format { get; set; }

    }
}