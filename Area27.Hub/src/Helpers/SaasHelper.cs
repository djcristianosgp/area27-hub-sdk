using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utility methods for common SaaS API operations.
/// </summary>
public static class SaasHelper
{
    /// <summary>
    /// Generates a secure API key for authentication.
    /// </summary>
    /// <param name="length">Length of the API key (default: 32).</param>
    /// <returns>A secure random API key in hexadecimal format.</returns>
    public static string GenerateApiKey(int length = 32)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than zero.", nameof(length));

        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return BitConverter.ToString(randomBytes).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Generates a unique token for password reset or email verification.
    /// </summary>
    /// <param name="length">Length of the token (default: 32).</param>
    /// <returns>A secure random token in hexadecimal format.</returns>
    public static string GenerateToken(int length = 32)
    {
        return GenerateApiKey(length);
    }

    /// <summary>
    /// Validates an email address format (simple validation).
    /// </summary>
    /// <param name="email">Email address to validate.</param>
    /// <returns><see langword="true"/> if email format is valid; otherwise <see langword="false"/>.</returns>
    public static bool IsValidEmail(string? email)
    {
        return ValidationHelper.IsEmailValido(email);
    }

    /// <summary>
    /// Validates password strength based on common criteria.
    /// </summary>
    /// <param name="password">Password to validate.</param>
    /// <param name="minLength">Minimum password length (default: 8).</param>
    /// <returns>Object containing validation result and strength details.</returns>
    public static PasswordStrengthResult ValidatePasswordStrength(string? password, int minLength = 8)
    {
        var result = new PasswordStrengthResult();

        if (string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Reasons.Add("Password cannot be empty.");
            return result;
        }

        if (password.Length < minLength)
        {
            result.IsValid = false;
            result.Reasons.Add($"Password must be at least {minLength} characters long.");
        }

        var hasUpperCase = password.Any(char.IsUpper);
        var hasLowerCase = password.Any(char.IsLower);
        var hasDigits = password.Any(char.IsDigit);
        var hasSpecialChars = password.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch));

        result.HasUpperCase = hasUpperCase;
        result.HasLowerCase = hasLowerCase;
        result.HasDigits = hasDigits;
        result.HasSpecialCharacters = hasSpecialChars;

        var strengthScore = (hasUpperCase ? 1 : 0) + (hasLowerCase ? 1 : 0) +
                           (hasDigits ? 1 : 0) + (hasSpecialChars ? 1 : 0);

        result.Strength = strengthScore switch
        {
            0 or 1 => PasswordStrength.Weak,
            2 => PasswordStrength.Fair,
            3 => PasswordStrength.Good,
            4 => PasswordStrength.Strong,
            _ => PasswordStrength.Weak
        };

        if (result.Strength == PasswordStrength.Weak)
        {
            result.IsValid = false;
            if (!hasUpperCase) result.Reasons.Add("Password must contain at least one uppercase letter.");
            if (!hasLowerCase) result.Reasons.Add("Password must contain at least one lowercase letter.");
            if (!hasDigits) result.Reasons.Add("Password must contain at least one digit.");
            if (!hasSpecialChars) result.Reasons.Add("Password must contain at least one special character.");
        }

        if (result.Reasons.Count == 0)
            result.IsValid = true;

        return result;
    }

    /// <summary>
    /// Hashes a password using PBKDF2.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <param name="saltSize">Salt size in bytes (default: 16).</param>
    /// <param name="iterations">PBKDF2 iterations (default: 10000).</param>
    /// <returns>Hashed password with salt included.</returns>
    public static string HashPassword(string password, int saltSize = 16, int iterations = 10000)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var salt = new byte[saltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var hashWithSalt = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, hashWithSalt, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, hashWithSalt, salt.Length, hash.Length);

        return Convert.ToBase64String(hashWithSalt);
    }

    /// <summary>
    /// Verifies a password against its hash.
    /// </summary>
    /// <param name="password">Password to verify.</param>
    /// <param name="hash">Hash to compare against.</param>
    /// <param name="iterations">PBKDF2 iterations (must match the one used in HashPassword).</param>
    /// <returns><see langword="true"/> if password matches the hash; otherwise <see langword="false"/>.</returns>
    public static bool VerifyPassword(string password, string hash, int iterations = 10000)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            const int saltSize = 16;

            var salt = new byte[saltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, saltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[saltSize + i] != computedHash[i])
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a subscription license key.
    /// </summary>
    /// <param name="customerId">Customer ID.</param>
    /// <param name="expirationDate">License expiration date.</param>
    /// <returns>License key string.</returns>
    public static string GenerateLicenseKey(string customerId, DateTime expirationDate)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

        var data = $"{customerId}|{expirationDate:yyyy-MM-dd}|{GenerateToken(16)}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
    }

    /// <summary>
    /// Validates a license key.
    /// </summary>
    /// <param name="licenseKey">License key to validate.</param>
    /// <param name="customerId">Expected customer ID.</param>
    /// <returns><see langword="true"/> if license is valid and not expired; otherwise <see langword="false"/>.</returns>
    public static bool ValidateLicenseKey(string licenseKey, string customerId)
    {
        if (string.IsNullOrWhiteSpace(licenseKey) || string.IsNullOrWhiteSpace(customerId))
            return false;

        try
        {
            var data = Encoding.UTF8.GetString(Convert.FromBase64String(licenseKey));
            var parts = data.Split('|');

            if (parts.Length != 3 || parts[0] != customerId)
                return false;

            if (!DateTime.TryParse(parts[1], out var expirationDate))
                return false;

            return DateTime.Now <= expirationDate;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a slug from a text (useful for URLs).
    /// </summary>
    /// <param name="text">Text to convert to slug.</param>
    /// <returns>URL-friendly slug.</returns>
    public static string GenerateSlug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var slug = text.ToLowerInvariant().Trim();
        
        // Remove accents
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(slug);
        slug = Encoding.ASCII.GetString(bytes);
        
        // Replace special characters with hyphens
        var chars = slug.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (!char.IsLetterOrDigit(chars[i]) && chars[i] != '-')
                chars[i] = '-';
        }

        slug = new string(chars);
        
        // Remove multiple consecutive hyphens
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");

        // Remove hyphens from start and end
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Truncates text with ellipsis.
    /// </summary>
    /// <param name="text">Text to truncate.</param>
    /// <param name="maxLength">Maximum length.</param>
    /// <param name="ellipsis">Ellipsis string (default: "...").</param>
    /// <returns>Truncated text with ellipsis.</returns>
    public static string Truncate(string? text, int maxLength, string ellipsis = "...")
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length <= maxLength)
            return text;

        return text[..(maxLength - ellipsis.Length)] + ellipsis;
    }

    /// <summary>
    /// Generates a subscription plan code.
    /// </summary>
    /// <param name="planName">Plan name.</param>
    /// <returns>Unique plan code.</returns>
    public static string GeneratePlanCode(string planName)
    {
        if (string.IsNullOrWhiteSpace(planName))
            throw new ArgumentException("Plan name cannot be empty.", nameof(planName));

        var slug = GenerateSlug(planName);
        var code = $"{slug}-{DateTime.Now.Ticks % 10000}";
        return code;
    }
}

/// <summary>
/// Represents password strength levels.
/// </summary>
public enum PasswordStrength
{
    /// <summary>Weak password</summary>
    Weak,
    /// <summary>Fair password</summary>
    Fair,
    /// <summary>Good password</summary>
    Good,
    /// <summary>Strong password</summary>
    Strong
}

/// <summary>
/// Contains password validation results.
/// </summary>
public class PasswordStrengthResult
{
    /// <summary>
    /// Indicates whether the password is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Password strength level.
    /// </summary>
    public PasswordStrength Strength { get; set; }

    /// <summary>
    /// Reasons for validation failure (if any).
    /// </summary>
    public List<string> Reasons { get; set; } = new();

    /// <summary>
    /// Indicates if password contains uppercase letters.
    /// </summary>
    public bool HasUpperCase { get; set; }

    /// <summary>
    /// Indicates if password contains lowercase letters.
    /// </summary>
    public bool HasLowerCase { get; set; }

    /// <summary>
    /// Indicates if password contains digits.
    /// </summary>
    public bool HasDigits { get; set; }

    /// <summary>
    /// Indicates if password contains special characters.
    /// </summary>
    public bool HasSpecialCharacters { get; set; }
}
