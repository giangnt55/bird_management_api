using Microsoft.IdentityModel.Tokens;

namespace AppCore.Extensions;

public static class DatetimeExtension
{
    public static long NowSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static long TotalSeconds(this DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime);
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    public static DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }

    public static DateTimeOffset Now(string timeZoneId)
    {
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, timeZoneId);
    }

    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddMilliseconds(-1);
    }
    public static DateTime ToLocal(this DateTime dateTime, string convertToTimeZone)
    {
        if (convertToTimeZone.IsNullOrEmpty())
            convertToTimeZone = "UTC";

        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime;

        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(convertToTimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, sourceTimeZone);
    }

    public static DateTime ToUtc(this DateTime dateTime, string sourceTimeZoneId)
    {
        if (sourceTimeZoneId.IsNullOrEmpty())
            sourceTimeZoneId = "UTC";
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime;
        }

        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
    }
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = dateTime.DayOfWeek - startOfWeek;
        if (diff < 0)
        {
            diff += 7;
        }

        return dateTime.AddDays(-1 * diff).Date;
    }
    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek endOfWeek = DayOfWeek.Sunday)
    {
        if (dateTime.DayOfWeek == endOfWeek)
        {
            return dateTime.Date.Date.AddDays(1).AddMilliseconds(-1);
        }
        else
        {
            var diff = dateTime.DayOfWeek - endOfWeek;
            return dateTime.AddDays(7 - diff).Date.AddDays(1).AddMilliseconds(-1);
        }
    }
}