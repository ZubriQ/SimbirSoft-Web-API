namespace SimbirSoft_Web_API.Helpers.Validators;

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
        return ids.All(id => IsValid(id));
    }
}
