namespace RealEstateSolution.Common.Utils;

/// <summary>
/// 日期时间工具类
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// 获取当前时间戳（毫秒）
    /// </summary>
    public static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 时间戳转DateTime
    /// </summary>
    public static DateTime TimestampToDateTime(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
    }

    /// <summary>
    /// DateTime转时间戳
    /// </summary>
    public static long DateTimeToTimestamp(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取友好的时间显示
    /// </summary>
    public static string GetFriendlyTime(DateTime dateTime)
    {
        var span = DateTime.Now - dateTime;
        if (span.TotalDays > 365)
        {
            return $"{(int)(span.TotalDays / 365)}年前";
        }
        if (span.TotalDays > 30)
        {
            return $"{(int)(span.TotalDays / 30)}个月前";
        }
        if (span.TotalDays > 1)
        {
            return $"{(int)span.TotalDays}天前";
        }
        if (span.TotalHours > 1)
        {
            return $"{(int)span.TotalHours}小时前";
        }
        if (span.TotalMinutes > 1)
        {
            return $"{(int)span.TotalMinutes}分钟前";
        }
        return "刚刚";
    }
} 