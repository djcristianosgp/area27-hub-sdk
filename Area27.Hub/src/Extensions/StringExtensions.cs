using System.Text;

namespace Area27.Hub.Extensions;

/// <summary>
/// Provides convenience extensions for working with strings safely.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Determines whether a string is null, empty, or whitespace safely.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <returns><see langword="true"/> when <paramref name="value"/> is null, empty, or whitespace; otherwise <see langword="false"/>.</returns>
    public static bool IsNullOrEmptySafe(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Returns a string containing only the numeric characters from the original value.
    /// </summary>
    /// <param name="value">The string to filter.</param>
    /// <returns>A string with only numeric characters; returns an empty string when <paramref name="value"/> is null.</returns>
    public static string OnlyNumbers(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (char.IsDigit(c))
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
