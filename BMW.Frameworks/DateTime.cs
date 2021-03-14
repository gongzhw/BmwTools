using System;
using System.Globalization;

namespace BMW.Frameworks
{
    public class DateTimeTool
    {
        private DateTimeTool()
        {
            // Nothing to do
        }
        
        /// <summary>
        /// 获取给定日期的周一
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetWeekFristDate(DateTime dt)
        {
            return DateTime.Parse(dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d"))).ToShortDateString());  //本周周一
        }

        /// <summary>
        /// 获取给定日期的周末
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetWeekEndDate(DateTime dt)
        {
            DateTime startWeek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));

            return DateTime.Parse(startWeek.AddDays(6).ToShortDateString());
        }

        /// <summary>
        /// 本月月初
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMonthFristDate(DateTime dt)
        {
            DateTime dts = dt.AddDays(1 - dt.Day);

            return DateTime.Parse(dts.ToShortDateString());
        }

        /// <summary>
        /// 本月月末
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMonthEndDate(DateTime dt)
        {
            DateTime dts = dt.AddDays(1 - dt.Day);

            return DateTime.Parse(dts.AddMonths(1).AddDays(-1).ToShortDateString());
        }

        /// <summary>
        /// 获取当前时间是第X周
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(DateTime dt)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }


        private DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);

            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 获取日期是周几
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToDayOfWeek(DateTime data, WeekStyle style)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string[] weekdays1 = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
        
            if(style == WeekStyle.周几)
            {
                return weekdays1[Convert.ToInt32(data.DayOfWeek)];
            }
            else
            {
                return weekdays[Convert.ToInt32(data.DayOfWeek)];
            }
        }

        public enum WeekStyle
        {
            星期几,
            周几
        }
    }
}
