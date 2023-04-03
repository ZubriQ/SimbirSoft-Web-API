namespace Olymp_Project.Helpers.Validators
{
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

            return IsStartDateLessThanEndDate(query.StartDate.Value, query.EndDate.Value);
        }

        private static bool IsValidDateFormat(DateTime date)
        {
            return date.TimeOfDay == TimeSpan.Zero;
        }

        private static bool IsStartDateLessThanEndDate(DateTime startDate, DateTime endDate)
        {
            return startDate < endDate;
        }
    }
}
