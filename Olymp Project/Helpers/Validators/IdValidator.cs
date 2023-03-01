namespace Olymp_Project.Helpers.Validators
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

        public static bool IsValid(params long?[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (!IsValid(ids[i]))
                {
                    return false;
                }
            }
            return true;
        }

        //public static class IdValidator<T> where T : struct, IComparable<T>
        //{
        //    public static bool IsValid(T? id)
        //    {
        //        return id.HasValue && id.Value.CompareTo(default(T)) > 0;
        //    }

        //    public static bool IsValid(params T?[] ids)
        //    {
        //        for (int i = 0; i < ids.Length; i++)
        //        {
        //            if (!IsValid(ids[i]))
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //}
    }
}
