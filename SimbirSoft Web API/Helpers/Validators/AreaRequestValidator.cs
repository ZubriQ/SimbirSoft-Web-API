namespace SimbirSoft_Web_API.Helpers.Validators;

public static class AreaRequestValidator
{
    public static bool IsValid(AreaRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.AreaPoints is null)
        {
            return false;
        }

        if (!ValidatePoints(request.AreaPoints))
        {
            return false;
        }

        return true;
    }

    private static bool ValidatePoints(AreaPointDto[] areaPoints)
    {
        return areaPoints.Length >= 3 && areaPoints.All(LocationPointValidator.IsValid);
    }
}
