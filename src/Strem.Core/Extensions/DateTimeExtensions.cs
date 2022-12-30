namespace Strem.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetStartOfMonth(this DateTime date)
    { return new DateTime(date.Year, date.Month, 1).Date; }

    public static DateTime GetEndOfMonth(this DateTime date)
    { return date.GetStartOfMonth().AddMonths(1).AddMilliseconds(-1); }

    public static DateTime GetStartOfDay(this DateTime date)
    { return new DateTime(date.Year, date.Month, date.Day); }
    
    public static DateTime GetEndOfDay(this DateTime date)
    { return date.GetStartOfDay().AddDays(1).AddMilliseconds(-1); }
}