namespace SimbirSoft_Web_API.Utilities;

public static class DateTimeExtensions
{
    public static string ToISO8601(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("u").Replace(" ", "T");
    }
}
