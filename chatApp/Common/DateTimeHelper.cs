namespace ChatApp.Common
{
    public static class DateTimeHelper
    {
        public static string FormatTimestamp(DateTime dt)
        {
            return dt.ToString("dd MMM yyyy HH:mm");
        }

        public static string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            return dateTime.ToString("dd MMM");
        }
    }
}
