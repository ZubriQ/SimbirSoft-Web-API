namespace Olymp_Project.Helpers.Validators
{
    public static class PathRequestValidator
    {
        public static bool IsWeightValid(double? value)
        {
            return value.HasValue && value.Value > 0;
        }
    }
}
