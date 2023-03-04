namespace Olymp_Project.Helpers
{
    public static class DateTimeFormatHelper
    {
        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("u").Replace(" ", "T");
        }
    }
}
