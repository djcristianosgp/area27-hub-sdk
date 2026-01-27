using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utilities for word counting and text analysis.
/// </summary>
public static class WordCountHelper
{
    /// <summary>
    /// Counts the total number of words in a text.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <returns>The number of words found in the text.</returns>
    public static int CountWords(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        // Remove extra whitespace and split by word boundaries
        var words = Regex.Split(text.Trim(), @"\s+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();

        return words.Length;
    }

    /// <summary>
    /// Counts occurrences of specific words in a text.
    /// </summary>
    /// <param name="text">The text to search.</param>
    /// <param name="wordsToCount">Array of words to count.</param>
    /// <param name="caseSensitive">Whether the search should be case-sensitive.</param>
    /// <returns>Dictionary with word counts.</returns>
    public static Dictionary<string, int> CountSpecificWords(string? text, string[] wordsToCount, bool caseSensitive = false)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(text) || wordsToCount == null || wordsToCount.Length == 0)
        {
            foreach (var word in wordsToCount ?? Array.Empty<string>())
            {
                result[word] = 0;
            }
            return result;
        }

        // Initialize result dictionary
        foreach (var word in wordsToCount)
        {
            result[word] = 0;
        }

        var textToSearch = caseSensitive ? text : text.ToLowerInvariant();
        var words = Regex.Split(textToSearch.Trim(), @"\s+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();

        foreach (var searchWord in wordsToCount)
        {
            var wordToFind = caseSensitive ? searchWord : searchWord.ToLowerInvariant();
            var count = words.Count(w => w.Equals(wordToFind, StringComparison.Ordinal));
            result[searchWord] = count;
        }

        return result;
    }

    /// <summary>
    /// Counts occurrences of each unique word in the text.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <param name="caseSensitive">Whether the search should be case-sensitive.</param>
    /// <returns>Dictionary with word counts for all unique words.</returns>
    public static Dictionary<string, int> CountAllWords(string? text, bool caseSensitive = false)
    {
        var result = new Dictionary<string, int>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(text))
            return result;

        var textToProcess = caseSensitive ? text : text.ToLowerInvariant();
        var words = Regex.Split(textToProcess.Trim(), @"\s+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();

        foreach (var word in words)
        {
            if (result.ContainsKey(word))
                result[word]++;
            else
                result[word] = 1;
        }

        return result;
    }

    /// <summary>
    /// Gets statistics about the text including word count, unique words, and average word length.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <returns>Object containing text statistics.</returns>
    public static TextStatistics GetStatistics(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new TextStatistics { TotalWords = 0, UniqueWords = 0, AverageWordLength = 0 };
        }

        var words = Regex.Split(text.Trim(), @"\s+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();

        var uniqueWords = words.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var avgLength = words.Average(w => w.Length);

        return new TextStatistics
        {
            TotalWords = words.Length,
            UniqueWords = uniqueWords,
            AverageWordLength = Math.Round(avgLength, 2),
            LongestWord = words.OrderByDescending(w => w.Length).FirstOrDefault() ?? string.Empty,
            ShortestWord = words.OrderBy(w => w.Length).FirstOrDefault() ?? string.Empty
        };
    }
}

/// <summary>
/// Represents text statistics.
/// </summary>
public class TextStatistics
{
    /// <summary>
    /// Total number of words in the text.
    /// </summary>
    public int TotalWords { get; set; }

    /// <summary>
    /// Number of unique words in the text.
    /// </summary>
    public int UniqueWords { get; set; }

    /// <summary>
    /// Average length of words in the text.
    /// </summary>
    public double AverageWordLength { get; set; }

    /// <summary>
    /// The longest word found in the text.
    /// </summary>
    public string LongestWord { get; set; } = string.Empty;

    /// <summary>
    /// The shortest word found in the text.
    /// </summary>
    public string ShortestWord { get; set; } = string.Empty;
}
