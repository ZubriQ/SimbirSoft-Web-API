namespace Olymp_Project.Helpers.Validators
{
    public static class PagingValidator
    {
        public static bool IsValid(Paging paging)
        {
            return paging.From.HasValue && paging.From >= 0
                && paging.Size.HasValue && paging.Size > 0;
        }
    }
}
