namespace Catebi.Api.Data.Helpers;

public static class Converters
{
    public static DateOnly ToDateOnly(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.ToDateOnly();
        }
        else
        {
            return default;
        }
    }

    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

}
