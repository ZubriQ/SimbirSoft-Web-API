namespace Olymp_Project.Helpers.Validators
{
    public static class LocationDtoValidator
    {
        public static bool IsValid(LocationRequestDto location)
        {
            return location.Latitude.HasValue && location.Latitude >= -90 && location.Latitude <= 90
                && location.Longitude.HasValue && location.Longitude >= -180 && location.Longitude <= 180;
        }
    }
}
