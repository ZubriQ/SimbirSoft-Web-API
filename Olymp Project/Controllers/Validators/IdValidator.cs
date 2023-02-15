namespace Olymp_Project.Controllers.Validators
{
    public static class IdValidator
    {
        public static bool IsValid(int? id)
        {
            return id.HasValue && id > 0;
        }

        public static bool IsValid(long? id)
        {
            return id.HasValue && id > 0;
        }
    }
}
