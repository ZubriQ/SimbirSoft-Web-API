namespace Olymp_Project.Helpers.Validators
{
    public static class AreaRequestValidator
    {
        public static bool IsValid(AreaRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.AreaPoints is null)
            {
                return false;
            }

            return ValidatePoints(request.AreaPoints);
        }

        private static bool ValidatePoints(AreaPointsDto[] areaPoints)
        {
            return areaPoints.Length >= 3 && areaPoints.All(LocationPointValidator.IsValid);
        }
    }
}
