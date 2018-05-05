using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSchedulerControl.Chart
{
    public class DateTimeHelper
    {
        public static readonly SortedDictionary<DayOfWeek, string> ShortDays = new SortedDictionary<DayOfWeek, string>
        {
            {DayOfWeek.Sunday, "Do"},
            {DayOfWeek.Monday, "Lu"},
            {DayOfWeek.Tuesday, "Ma"},
            {DayOfWeek.Wednesday, "Mi"},
            {DayOfWeek.Thursday, "Ju"},
            {DayOfWeek.Friday, "Vi"},
            {DayOfWeek.Saturday, "Sa"}
        };
    }
}
