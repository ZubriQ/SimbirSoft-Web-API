﻿namespace Olymp_Project.Controllers.Validators
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

        public static bool IsValid(params int?[] ids)
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
    }
}
