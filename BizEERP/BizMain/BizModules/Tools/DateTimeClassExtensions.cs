using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ATL.DateTimeExt
{
    public static class DateTimeExtensions
    {
        public static int weekNumber(System.DateTime value)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
