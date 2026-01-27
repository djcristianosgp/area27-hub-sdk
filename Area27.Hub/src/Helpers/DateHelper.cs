using System;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utility methods for date manipulation and calculations.
/// </summary>
public static class DateHelper
{
    /// <summary>
    /// Enum defining the format of the result for date calculations.
    /// </summary>
    public enum DateResultFormat
    {
        /// <summary>Returns as grouped format: "X dias, Y meses, Z anos"</summary>
        Grouped,
        /// <summary>Returns as separated format: "dias=X, meses=Y, anos=Z"</summary>
        Separated
    }

    /// <summary>
    /// Calculates the difference between two dates.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="format">The format of the result (Grouped or Separated).</param>
    /// <returns>A string representing the difference between dates formatted as specified.</returns>
    /// <exception cref="ArgumentException">Thrown when startDate is greater than endDate.</exception>
    public static string CalculateDateDifference(DateTime startDate, DateTime endDate, DateResultFormat format = DateResultFormat.Grouped)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be less than or equal to end date.", nameof(startDate));

        var years = endDate.Year - startDate.Year;
        var months = endDate.Month - startDate.Month;
        var days = endDate.Day - startDate.Day;

        // Adjust if days is negative
        if (days < 0)
        {
            months--;
            days += DateTime.DaysInMonth(endDate.Year, endDate.Month);
        }

        // Adjust if months is negative
        if (months < 0)
        {
            years--;
            months += 12;
        }

        return format == DateResultFormat.Grouped
            ? $"{days} dias, {months} meses, {years} anos"
            : $"dias={days}, meses={months}, anos={years}";
    }

    /// <summary>
    /// Calculates the age based on birth date.
    /// </summary>
    /// <param name="birthDate">The birth date.</param>
    /// <param name="referenceDate">The reference date for calculation (default: today).</param>
    /// <param name="format">The format of the result (Grouped or Separated).</param>
    /// <returns>A string representing the age formatted as specified.</returns>
    /// <exception cref="ArgumentException">Thrown when birthDate is in the future.</exception>
    public static string CalculateAge(DateTime birthDate, DateTime? referenceDate = null, DateResultFormat format = DateResultFormat.Grouped)
    {
        referenceDate ??= DateTime.Now.Date;

        if (birthDate > referenceDate)
            throw new ArgumentException("Birth date cannot be in the future.", nameof(birthDate));

        return CalculateDateDifference(birthDate.Date, referenceDate.Value.Date, format);
    }

    /// <summary>
    /// Gets the age in years only.
    /// </summary>
    /// <param name="birthDate">The birth date.</param>
    /// <param name="referenceDate">The reference date for calculation (default: today).</param>
    /// <returns>The age in years.</returns>
    /// <exception cref="ArgumentException">Thrown when birthDate is in the future.</exception>
    public static int GetAgeInYears(DateTime birthDate, DateTime? referenceDate = null)
    {
        referenceDate ??= DateTime.Now.Date;

        if (birthDate > referenceDate)
            throw new ArgumentException("Birth date cannot be in the future.", nameof(birthDate));

        var age = referenceDate.Value.Year - birthDate.Year;
        if (birthDate.Date > referenceDate.Value.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// Adds a specified number of days to a date.
    /// </summary>
    /// <param name="date">The base date.</param>
    /// <param name="daysToAdd">The number of days to add (can be negative).</param>
    /// <returns>The new date after adding days.</returns>
    public static DateTime AddDays(DateTime date, int daysToAdd)
    {
        return date.AddDays(daysToAdd);
    }

    /// <summary>
    /// Adds a specified number of months to a date.
    /// </summary>
    /// <param name="date">The base date.</param>
    /// <param name="monthsToAdd">The number of months to add (can be negative).</param>
    /// <returns>The new date after adding months.</returns>
    public static DateTime AddMonths(DateTime date, int monthsToAdd)
    {
        return date.AddMonths(monthsToAdd);
    }

    /// <summary>
    /// Adds a specified number of years to a date.
    /// </summary>
    /// <param name="date">The base date.</param>
    /// <param name="yearsToAdd">The number of years to add (can be negative).</param>
    /// <returns>The new date after adding years.</returns>
    public static DateTime AddYears(DateTime date, int yearsToAdd)
    {
        return date.AddYears(yearsToAdd);
    }

    /// <summary>
    /// Adds years, months, and days to a date.
    /// </summary>
    /// <param name="date">The base date.</param>
    /// <param name="yearsToAdd">The number of years to add (can be negative).</param>
    /// <param name="monthsToAdd">The number of months to add (can be negative).</param>
    /// <param name="daysToAdd">The number of days to add (can be negative).</param>
    /// <returns>The new date after adding the specified period.</returns>
    public static DateTime AddPeriod(DateTime date, int yearsToAdd = 0, int monthsToAdd = 0, int daysToAdd = 0)
    {
        return date.AddYears(yearsToAdd).AddMonths(monthsToAdd).AddDays(daysToAdd);
    }

    /// <summary>
    /// Gets the number of days between two dates.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns>The absolute number of days between the two dates.</returns>
    public static int DaysBetween(DateTime startDate, DateTime endDate)
    {
        return Math.Abs((int)(endDate.Date - startDate.Date).TotalDays);
    }

    /// <summary>
    /// Checks if a year is a leap year.
    /// </summary>
    /// <param name="year">The year to check.</param>
    /// <returns><see langword="true"/> if the year is a leap year; otherwise <see langword="false"/>.</returns>
    public static bool IsLeapYear(int year)
    {
        return DateTime.IsLeapYear(year);
    }

    /// <summary>
    /// Gets the last day of the month for a given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A DateTime representing the last day of the month.</returns>
    public static DateTime GetLastDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    /// <summary>
    /// Gets the first day of the month for a given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A DateTime representing the first day of the month.</returns>
    public static DateTime GetFirstDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Calculates the number of business days between two dates (excluding weekends).
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns>The number of business days between the two dates.</returns>
    /// <exception cref="ArgumentException">Thrown when startDate is greater than endDate.</exception>
    public static int BusinessDaysBetween(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be less than or equal to end date.", nameof(startDate));

        int businessDays = 0;
        DateTime currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                businessDays++;

            currentDate = currentDate.AddDays(1);
        }

        return businessDays;
    }

    /// <summary>
    /// Gets the day of the week name in Portuguese.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The day name in Portuguese (e.g., "segunda-feira", "terça-feira").</returns>
    public static string GetDayNameInPortuguese(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => "domingo",
            DayOfWeek.Monday => "segunda-feira",
            DayOfWeek.Tuesday => "terça-feira",
            DayOfWeek.Wednesday => "quarta-feira",
            DayOfWeek.Thursday => "quinta-feira",
            DayOfWeek.Friday => "sexta-feira",
            DayOfWeek.Saturday => "sábado",
            _ => "desconhecido"
        };
    }

    /// <summary>
    /// Gets the month name in Portuguese.
    /// </summary>
    /// <param name="month">The month number (1-12).</param>
    /// <returns>The month name in Portuguese.</returns>
    /// <exception cref="ArgumentException">Thrown when month is not between 1 and 12.</exception>
    public static string GetMonthNameInPortuguese(int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Month must be between 1 and 12.", nameof(month));

        return month switch
        {
            1 => "janeiro",
            2 => "fevereiro",
            3 => "março",
            4 => "abril",
            5 => "maio",
            6 => "junho",
            7 => "julho",
            8 => "agosto",
            9 => "setembro",
            10 => "outubro",
            11 => "novembro",
            12 => "dezembro",
            _ => "desconhecido"
        };
    }

    /// <summary>
    /// Formats a date to a readable Brazilian format string.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>A formatted string like "27 de janeiro de 2026, segunda-feira".</returns>
    public static string FormatDateInPortuguese(DateTime date)
    {
        var dayName = GetDayNameInPortuguese(date);
        var monthName = GetMonthNameInPortuguese(date.Month);
        return $"{date.Day} de {monthName} de {date.Year}, {dayName}";
    }
}
