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

    private static readonly Random Random = new();

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

    /// <summary>
    /// Generates a valid CPF number.
    /// </summary>
    /// <param name="formatted">Whether to return the CPF formatted (xxx.xxx.xxx-xx).</param>
    /// <returns>A valid CPF number.</returns>
    public static string GenerateCpf(bool formatted = false)
    {
        // Generate 9 random digits
        var digits = new char[9];
        for (int i = 0; i < 9; i++)
        {
            digits[i] = (char)('0' + Random.Next(10));
        }

        var baseDigits = new string(digits);

        // Calculate check digits
        var firstCheck = CalculateCpfCheckDigit(baseDigits);
        var secondCheckBase = baseDigits + firstCheck;
        var secondCheck = CalculateCpfCheckDigit(secondCheckBase);

        var cpf = baseDigits + firstCheck + secondCheck;

        if (formatted)
        {
            return $"{cpf[..3]}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        return cpf;
    }

    /// <summary>
    /// Validates a CNPJ (Cadastro Nacional da Pessoa Jurídica) number using check-digit verification.
    /// </summary>
    /// <param name="cnpj">CNPJ value with or without punctuation.</param>
    /// <returns><see langword="true"/> when the CNPJ is structurally valid; otherwise <see langword="false"/>.</returns>
    public static bool IsCnpjValido(string? cnpj)
    {
        var digits = cnpj.OnlyNumbers();
        if (digits.Length != 14)
            return false;

        // Reject sequences with all the same digit
        if (digits.All(d => d == digits[0]))
            return false;

        var first  = CalculateCnpjCheckDigit(digits[..12], new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
        var second = CalculateCnpjCheckDigit(digits[..13], new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

        return digits[12] == first && digits[13] == second;
    }

    private static char CalculateCnpjCheckDigit(string baseDigits, int[] weights)
    {
        var sum = 0;
        for (int i = 0; i < baseDigits.Length; i++)
            sum += (baseDigits[i] - '0') * weights[i];

        var remainder = sum % 11;
        return (char)('0' + (remainder < 2 ? 0 : 11 - remainder));
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
