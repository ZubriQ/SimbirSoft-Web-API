namespace Olymp_Project.Controllers.Validators
{
    public static class IdValidator
    {
        public static bool IsValid(int? value)
        {
            return value.HasValue && value > 0;
        }

        public static bool IsValid(long? value)
        {
            return value.HasValue && value > 0;
        }
    }
}
