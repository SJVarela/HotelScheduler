using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSchedulerControl.Chart
{
    public class HeaderInfo
    {
        public RectangleF H1Rect;
        public RectangleF H2Rect;
        public List<RectangleF> LabelRects;
        public List<RectangleF> Columns;
        public List<DateTime> DateTimes;
    }
}
