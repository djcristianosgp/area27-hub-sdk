using System;
using System.Linq;
using System.Text.RegularExpressions;
using Area27.Hub.Extensions;

namespace Area27.Hub.Helpers;

/// <summary>
/// Offers validation helpers for common Brazilian identifiers and email addresses.
/// </summary>
public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new(
        pattern: @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Validates a CPF (Cadastro de Pessoas Físicas) number.
    /// </summary>
    /// <param name="cpf">CPF value with or without punctuation.</param>
    /// <returns><see langword="true"/> when the CPF is structurally valid; otherwise <see langword="false"/>.</returns>
    public static bool IsCpfValido(string? cpf)
    {
        var digits = cpf.OnlyNumbers();
        if (digits.Length != 11)
        {
            return false;
        }

        // Reject sequences with the same digit (e.g., 11111111111)
        if (digits.All(d => d == digits[0]))
        {
            return false;
        }

        var firstCheck = CalculateCpfCheckDigit(digits[..9]);
        var secondCheck = CalculateCpfCheckDigit(digits[..10]);

        return digits[9] == firstCheck && digits[10] == secondCheck;
    }

    /// <summary>
    /// Validates a simple email address format.
    /// </summary>
    /// <param name="email">Email address to validate.</param>
    /// <returns><see langword="true"/> when the email matches a basic pattern; otherwise <see langword="false"/>.</returns>
    public static bool IsEmailValido(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email);
    }

    private static char CalculateCpfCheckDigit(string baseDigits)
    {
        var sum = 0;
        for (int i = 0; i < baseDigits.Length; i++)
        {
            var digit = baseDigits[i] - '0';
            var weight = baseDigits.Length + 1 - i;
            sum += digit * weight;
        }

        var remainder = sum % 11;
        var result = remainder < 2 ? 0 : 11 - remainder;
        return (char)('0' + result);
    }
}
