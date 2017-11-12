using System;

namespace PluginCore.Extensions
{
    public static class DateExtensions
    {
        public static DateTime GetEndOfDay(DateTime dateToConsider)
        {
            return new DateTime(dateToConsider.Year, dateToConsider.Month, dateToConsider.Day, 23, 59, 59);
        }
        public static DateTime GetStartOfDay(DateTime dateToConsider)
        {
            return new DateTime(dateToConsider.Year, dateToConsider.Month, dateToConsider.Day, 0, 0, 0);
        }
    }
}
