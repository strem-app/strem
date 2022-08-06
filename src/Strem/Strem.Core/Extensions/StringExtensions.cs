using System.Text.RegularExpressions;
using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class StringExtensions
{
    public static bool MatchesText(this string value, TextMatch matchType, string matchText)
    {
        return matchType switch
        {
            TextMatch.None => true,
            TextMatch.Contains => value.Contains(matchText),
            TextMatch.StartsWith => value.StartsWith(matchText),
            TextMatch.EndsWith => value.EndsWith(matchText),
            _ => Regex.IsMatch(value, matchText)
        };
    }
}