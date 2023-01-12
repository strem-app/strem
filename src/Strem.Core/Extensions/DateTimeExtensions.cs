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
    
    public static DateTime ToSecondAccuracy(this DateTime datetime)
    { return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second); }

    public static DateTime ToMinuteAccuracy(this DateTime datetime)
    { return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0); }
    
    public static DateTime ToHourAccuracy(this DateTime datetime)
    { return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0); }
    
    public static DateTime ToDayAccuracy(this DateTime datetime)
    { return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0); }
    
    public static DateTime ToMonthAccuracy(this DateTime datetime)
    { return new DateTime(datetime.Year, datetime.Month, 1, 0, 0, 0); }
    
    public static DateTime ToYearAccuracy(this DateTime datetime)
    { return new DateTime(datetime.Year, 1, 1, 0, 0, 0); }
    
    public static DateTime RoundToNearest(this DateTime datetime, TimeSpan rounding)
    {
        var delta = datetime.Ticks % rounding.Ticks;
        var roundUp = delta > rounding.Ticks / 2;
        var offset = roundUp ? rounding.Ticks : 0;
        return new DateTime(datetime.Ticks + offset - delta, datetime.Kind);
    }
}