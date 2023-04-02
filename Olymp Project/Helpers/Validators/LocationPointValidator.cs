namespace Olymp_Project.Helpers.Validators
{
    public static class LocationPointValidator
    {
        private static bool IsValidCoordinate(double? latitude, double? longitude)
        {
            return latitude.HasValue && latitude >= -90 && latitude <= 90
                && longitude.HasValue && longitude >= -180 && longitude <= 180;
        }

        public static bool IsValid(LocationRequestDto location)
        {
            return IsValidCoordinate(location.Latitude, location.Longitude);
        }

        public static bool IsValid(AreaPointsDto point)
        {
            return IsValidCoordinate(point.Latitude, point.Longitude);
        }
    }
}
