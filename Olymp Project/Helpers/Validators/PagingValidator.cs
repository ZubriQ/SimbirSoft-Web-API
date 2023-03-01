namespace Olymp_Project.Helpers.Validators
{
    public static class PagingValidator
    {
        public static bool IsValid(Paging paging)
        {
            return paging.Skip.HasValue && paging.Skip >= 0 
                && paging.Take.HasValue && paging.Take > 0;
        }
    }
}
