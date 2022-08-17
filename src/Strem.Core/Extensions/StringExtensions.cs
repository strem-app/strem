using System.Text.RegularExpressions;
using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class StringExtensions
{
    public static bool MatchesText(this string? value, TextMatchType matchType, string? matchText)
    {
        if (value == null) { return matchType == TextMatchType.None; }
        if (matchText == null) { return matchType == TextMatchType.None; }
        
        return matchType switch
        {
            TextMatchType.None => true,
            TextMatchType.Contains => value.Contains(matchText),
            TextMatchType.StartsWith => value.StartsWith(matchText),
            TextMatchType.EndsWith => value.EndsWith(matchText),
            TextMatchType.ExactMatch => value == matchText,
            _ => Regex.IsMatch(value, matchText)
        };
    }
    
    public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
    {
        return value?.Length > maxLength
            ? value.Substring(0, maxLength) + truncationSuffix
            : value;
    }
}