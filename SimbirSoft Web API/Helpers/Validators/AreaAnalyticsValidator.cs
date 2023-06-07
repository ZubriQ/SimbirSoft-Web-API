namespace SimbirSoft_Web_API.Helpers.Validators;

public static class AreaAnalyticsValidator
{
    public static bool IsValid(AreaAnalyticsQuery query)
    {
        if (query.StartDate == null || query.EndDate == null)
        {
            return false;
        }

        if (!IsValidDateFormat(query.StartDate.Value) || !IsValidDateFormat(query.EndDate.Value))
        {
            return false;
        }

        if (query.StartDate.Value >= query.EndDate.Value)
        {
            return false;
        }

        return true;
    }

    private static bool IsValidDateFormat(DateTime date)
    {
        return date.TimeOfDay == TimeSpan.Zero;
    }
}
