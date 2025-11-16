using System.Globalization;

namespace Lingrow.BusinessLogicLayer.Helper;

public static class ValueParser
{
    /// <summary>
    /// Parse DateOnly? từ string (format yyyy-MM-dd).
    /// </summary>
    public static DateOnly? ParseDateOnly(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (
            DateOnly.TryParseExact(
                input,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dob
            )
        )
        {
            return dob;
        }

        LoggerHelper.Warn($"Cannot parse DateOnly from: {input}");
        return null;
    }

    /// <summary>
    /// Parse int? từ string.
    /// </summary>
    public static int? ParseInt(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (int.TryParse(input, out var value))
            return value;

        LoggerHelper.Warn($"Cannot parse int from: {input}");
        return null;
    }

    /// <summary>
    /// Parse double? từ string.
    /// </summary>
    public static double? ParseDouble(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            return value;

        LoggerHelper.Warn($"Cannot parse double from: {input}");
        return null;
    }

    /// <summary>
    /// Parse bool? từ string ("true"/"false"/"1"/"0").
    /// </summary>
    public static bool? ParseBool(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (bool.TryParse(input, out var b))
            return b;

        if (input == "1")
            return true;
        if (input == "0")
            return false;

        LoggerHelper.Warn($"Cannot parse bool from: {input}");
        return null;
    }
}
